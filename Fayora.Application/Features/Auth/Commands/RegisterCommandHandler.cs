using Fayora.Application.Common.Authentication;
using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging; // 👈 ضفنا الـ Namespace ده

namespace Fayora.Application.Features.Auth.Commands;

public class RegisterCommandHandler(
    IPasswordHasher passwordHasher,
    IDeviceRepository deviceRepository,
    IRefreshTokenService refreshTokenService,
    IJwtTokenService jwtTokenService,
    IClientContextProvider contextProvider,
    IUserRepository userRepository,
    IRefreshTokensRepository refreshTokensRepository,
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var context = contextProvider.GetContext();

            if (string.IsNullOrWhiteSpace(context.DeviceId))
            {
                return UserErrors.DeviceIdMissing;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                if (await userRepository.IsEmailExistAsync(request.Email, cancellationToken))
                    return UserErrors.EmailAlreadyExists;
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (await userRepository.IsPhoneExistAsync(request.PhoneNumber, cancellationToken))
                    return UserErrors.PhoneAlreadyExists;
            }

            var passwordHash = passwordHasher.Hash(request.Password);

            var userResult = User.Create(request.FirstName, request.LastName, request.Email, request.PhoneNumber, passwordHash, request.SimCountryIsoCode, request.DeviceInfo.DeviceLanguage, request.TimeZone);

            if (userResult.IsError)
            {
                return userResult.Errors;
            }

            var user = userResult.Value;
            var jwtToken = jwtTokenService.GenerateToken(user);
            var refreshTokenString = refreshTokenService.GenerateTokenString();
            var refreshToken = new RefreshToken(user.Id, refreshTokenString, context.DeviceId, context.IpAddress);

            var device = new UserDevice(user.Id, context.DeviceId, request.DeviceInfo.FcmToken, request.DeviceInfo.DeviceType, request.DeviceInfo.DeviceModel, request.DeviceInfo.DeviceLanguage);

            await userRepository.AddUserAsync(user, cancellationToken);
            await deviceRepository.AddDeviceAsync(device, cancellationToken);
            await refreshTokensRepository.AddTokenAsync(refreshToken, cancellationToken);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new AuthResult(user.Id, user.FirstName, user.LastName, user.PrimaryEmail?.Value, user.PhoneNumber, jwtToken, refreshTokenString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while registering user {Email} or {Phone}", request.Email, request.PhoneNumber);

            return Error.Failure("Registration.Failed", "An error occurred during registration. Please try again later.");
        }
    }
}