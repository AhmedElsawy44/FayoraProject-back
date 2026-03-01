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
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (await bannedItemRepository.IsBannedAsync(BanType.DeviceId, request.DeviceId, cancellationToken))
                return UserErrors.DeviceBanned;

            if (IsProvided(request.Email))
            {
                if (await bannedItemRepository.IsBannedAsync(BanType.Email, request.Email!, cancellationToken))
                    return UserErrors.EmailBanned;

                if (await userRepository.IsEmailExistAsync(request.Email!, cancellationToken))
                    return UserErrors.EmailAlreadyExists;
            }

            if (IsProvided(request.PhoneNumber))
            {
                if (await bannedItemRepository.IsBannedAsync(BanType.PhoneNumber, request.PhoneNumber!, cancellationToken))
                    return UserErrors.PhoneBanned;

                if (await userRepository.IsPhoneExistAsync(request.PhoneNumber!, cancellationToken))
                    return UserErrors.PhoneAlreadyExists;
            }

            var passwordHashResult = passwordHasher.HashPassword(request.Password);

            if (passwordHashResult.IsError)
            {
                return UserErrors.InvalidPassword;
            }


            var userResult = User.Create(request.Email, request.PhoneNumber, passwordHashResult.Value, request.SimCountryIsoCode, request.DeviceLanguage, request.TimeZone);

            if (userResult.IsError)
            {
                return userResult.Errors;
            }

            var user = userResult.Value;
            var jwtToken = jwtTokenService.GenerateToken(request.DeviceId, user);
            var refreshTokenString = refreshTokenService.GenerateTokenString();
            var refreshToken = new RefreshToken(user.Id, refreshTokenString, request.DeviceId, contextProvider.GetContext().IpAddress);

            var target = IsProvided(request.Email) ? request.Email : request.PhoneNumber;
            var vCode = codeService.GenerateCode();
            var vCodeHash = codeHasher.HashCode(vCode);
            var codeType = IsProvided(request.Email) ? CodeType.Email : CodeType.SMS;

            var otpResult = user.RequestOtp(target!, vCode, codeType, vCodeHash, OtpPurpose.Registration);
            if (otpResult.IsError)
            {
                return otpResult.Errors;
            }

            userRepository.AddUser(user);
            refreshTokensRepository.AddToken(refreshToken);
            await unitOfWork.CommitChangesAsync(cancellationToken);


            return new AuthResult(user.Id, user.PrimaryEmail?.Value, user.PhoneNumber, jwtToken, refreshTokenString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            logger.LogError(ex, "An error occurred while registering user {Email} or {Phone}", request.Email, request.PhoneNumber);

            return Error.Failure("Registration.Failed", "An error occurred during registration.");
        }
    }
    static bool IsProvided(string? text) => !string.IsNullOrWhiteSpace(text);
}