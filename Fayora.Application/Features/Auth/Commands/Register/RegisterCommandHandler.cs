using Fayora.Application.Common.Authentication;
using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fayora.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IPasswordHasher passwordHasher,
    IRefreshTokenService refreshTokenService,
    IJwtService jwtTokenService,
    IClientContextProvider contextProvider,
    IVerificationCodeService codeService,
    ICodeHasher codeHasher,
    IBannedItemRepository bannedItemRepository,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokensRepository,
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // ✨ Watch this video: https://www.youtube.com/watch?v=wpbFS6OC5bA

            var deviceIdBannedTask = bannedItemRepository.IsBannedAsync(BanType.DeviceId, request.DeviceId, cancellationToken);

            var emailBannedTask = !string.IsNullOrWhiteSpace(request.Email)
                ? bannedItemRepository.IsBannedAsync(BanType.Email, request.Email, cancellationToken)
                : Task.FromResult(false);

            var phoneBannedTask = !string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? bannedItemRepository.IsBannedAsync(BanType.PhoneNumber, request.PhoneNumber, cancellationToken)
                : Task.FromResult(false);

            var emailCheckTask = !string.IsNullOrWhiteSpace(request.Email)
                ? userRepository.IsEmailExistAsync(request.Email, cancellationToken)
                : Task.FromResult(false);

            var phoneCheckTask = !string.IsNullOrWhiteSpace(request.PhoneNumber)
                ? userRepository.IsPhoneExistAsync(request.PhoneNumber, cancellationToken)
                : Task.FromResult(false);

            await Task.WhenAll(deviceIdBannedTask, emailBannedTask, phoneBannedTask, emailCheckTask, phoneCheckTask);

            if (await phoneBannedTask) return UserErrors.PhoneBanned;
            if (await deviceIdBannedTask) return UserErrors.DeviceBanned;
            if (await emailBannedTask) return UserErrors.EmailBanned;

            if (await emailCheckTask) return UserErrors.EmailAlreadyExists;
            if (await phoneCheckTask) return UserErrors.PhoneAlreadyExists;

            var passwordHashResult = passwordHasher.HashPassword(request.Password);

            if(passwordHashResult.IsError)
            {
                return UserErrors.InvalidPassword;
            }


            var userResult = User.Create(request.FirstName, request.LastName, request.Email, request.PhoneNumber, passwordHashResult.Value, request.SimCountryIsoCode, request.DeviceLanguage, request.TimeZone);

            if (userResult.IsError)
            {
                return userResult.Errors;
            }

            var user = userResult.Value;
            var jwtToken = jwtTokenService.GenerateToken(request.DeviceId, user);
            var refreshTokenString = refreshTokenService.GenerateTokenString();
            var refreshToken = new RefreshToken(user.Id, refreshTokenString, request.DeviceId, contextProvider.GetContext().IpAddress);

            var existDevice = await deviceRepository.GetDeviceByDeviceIdAsync(request.DeviceId, cancellationToken, IsTracking: true);
            if (existDevice != null && existDevice.FCMToken != request.FcmToken)
            {
                existDevice.UpdateFcmToken(request.FcmToken);
            }
            else
            {
                var device = new UserDevice(user.Id, request.DeviceId, request.FcmToken, request.DeviceType, request.DeviceModel, request.DeviceLanguage);
                deviceRepository.AddDevice(device); 
            }


            var target = !string.IsNullOrWhiteSpace(request.Email) ? request.Email : request.PhoneNumber;
            var vCode = codeService.GenerateCode();
            var vCodeHash = codeHasher.HashCode(vCode);
            var codeType = !string.IsNullOrWhiteSpace(request.Email) ? CodeType.Email : CodeType.SMS;

            var otpResult = user.RequestOtp(target!, vCodeHash, codeType, vCode, OtpPurpose.Registration);
            if (otpResult.IsError)
            {
                return otpResult.Errors;
            }

            userRepository.AddUser(user);
            refreshTokensRepository.AddToken(refreshToken);
            await unitOfWork.CommitChangesAsync(cancellationToken);


            return new AuthResult(user.Id, user.FirstName, user.LastName, user.PrimaryEmail?.Value, user.PhoneNumber, jwtToken, refreshTokenString);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while registering user {Email} or {Phone}", request.Email, request.PhoneNumber);

            return Error.Failure("Registration.Failed", "An error occurred during registration.");
        }
    }
}