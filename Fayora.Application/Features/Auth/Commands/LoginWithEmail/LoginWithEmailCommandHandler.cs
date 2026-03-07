using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entitties.Identity;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.LoginWithEmail
{
    public class LoginWithEmailCommandHandler(IUserRepository userRepository, IUserTokenRepository userTokenRepository, IDeviceRepository deviceRepository, IUnitOfWork unitOfWork, IJwtService jwtService, ITokenHasher tokenHasher, IUserTokenService userTokenService, IPasswordHasher passwordHasher, IClientContextProvider clientContextProvider) : IRequestHandler<LoginWithEmailCommand, Result<LoginWithEmailResult>>
    {
        public async Task<Result<LoginWithEmailResult>> Handle(LoginWithEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByEmailAsync(request.Email, new UserQueryOptions { IsReadOnly = false, IncludeRoles = true }, cancellationToken);

            if (user is null) return AuthErrors.InvalidCredentials;

            var statusCheck = user.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;

            if (!user.IsVerified) return AuthErrors.UserNotVerified;

            if (!user.IsCorrectPasswordHash(request.Password, passwordHasher))
            {
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return AuthErrors.InvalidCredentials;
            }

            user.Login();

            var device = await deviceRepository.GetDeviceByUserIdAndDeviceIdAsync(user.Id, request.DeviceId, cancellationToken, isTracking: true);

            if (device is null)
            {
                device = new UserDevice(user.Id, request.DeviceId, request.FcmToken, request.DeviceLanguage);
                deviceRepository.AddDevice(device);
            }
            else
            {
                device.UpdateInfo(request.FcmToken, request.DeviceLanguage);
            }

            var accessToken = jwtService.GenerateToken(request.DeviceId, user);

            var refreshTokenString = userTokenService.GenerateTokenString();

            var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);

            var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, request.DeviceId, clientContextProvider.GetContext().IpAddress);

            await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, request.DeviceId, TokenType.RefreshToken, cancellationToken);

            userTokenRepository.AddToken(refreshToken);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new LoginWithEmailResult(user.Id, request.Email, accessToken, refreshTokenString, jwtService.ExpiresIn);
        }
    }
}
