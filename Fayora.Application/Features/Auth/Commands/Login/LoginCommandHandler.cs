using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IUserRepository userRepository, IUserTokenRepository userTokenRepository, IDeviceRepository deviceRepository, IUnitOfWork unitOfWork, IJwtService jwtService, ITokenHasher tokenHasher, IUserTokenService userTokenService, IPasswordHasher passwordHasher, IClientContextProvider clientContextProvider) : IRequestHandler<LoginCommand, Result<AuthResult>>
{

    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdentityAsync(request.Identity, new UserQueryOptions { IsTracking = true, IncludeRoles = true }, cancellationToken);

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

        var roles = user.GetRoleNames();

        var accessToken = jwtService.GenerateToken(request.DeviceId, user, roles);

        var refreshTokenString = userTokenService.GenerateTokenString();

        var hashedRefreshToken = tokenHasher.HashToken(refreshTokenString);

        var refreshToken = UserTokens.RefreshToken(user.Id, hashedRefreshToken, request.DeviceId, clientContextProvider.GetContext().IpAddress);

        await userTokenRepository.RevokeTokensForDeviceAsync(user.Id, request.DeviceId, TokenType.RefreshToken, cancellationToken);

        userTokenRepository.AddToken(refreshToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new AuthResult(user.Id, request.Identity, accessToken, refreshTokenString);
    }
}
