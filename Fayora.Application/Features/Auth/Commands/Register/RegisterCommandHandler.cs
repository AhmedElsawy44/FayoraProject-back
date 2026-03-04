using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.Identity;
using Fayora.Domain.Enums;
using Fayora.Domain.Errors;
using Fayora.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using static Fayora.Application.Common.Interfaces.Presistance.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(
    IPasswordHasher passwordHasher,
    IVerificationCodeService codeService,
    ICodeHasher codeHasher,
    IBannedItemRepository bannedItemRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<RegisterResult>>
{
    public async Task<Result<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (await bannedItemRepository.IsBannedAsync(BanType.DeviceId, request.DeviceId, cancellationToken))
                return UserErrors.DeviceBanned;

            bool isEmailRegistration = IsProvided(request.Email);
            string identity = isEmailRegistration ? request.Email! : request.PhoneNumber!;
            BanType identityBanType = isEmailRegistration ? BanType.Email : BanType.PhoneNumber;

            if (await bannedItemRepository.IsBannedAsync(identityBanType, identity, cancellationToken))
                return isEmailRegistration ? UserErrors.EmailBanned : UserErrors.PhoneBanned;

            var options = new UserQueryOptions { IsTracking = true };

            var existUser = await userRepository.GetUserByIdentityAsync(identity, options, cancellationToken);

            if (existUser != null)
            {
                bool isVerified = isEmailRegistration ? existUser.IsEmailVerified : existUser.IsPhoneVerified;

                if (isVerified)
                    return isEmailRegistration ? UserErrors.EmailAlreadyExists : UserErrors.PhoneAlreadyExists;

                var accountOtpResult = existUser.RequestOtp(identity, OtpPurpose.Registration, codeService, codeHasher);
                if (accountOtpResult.IsError) return accountOtpResult.Errors;

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new RegisterResult(existUser.Id, existUser.PrimaryEmail?.Value, existUser.PhoneNumber);
            }

            var userResult = User.Create(
                request.Email,
                request.PhoneNumber,
                request.Password,
                request.SimCountryIsoCode,
                request.DeviceLanguage,
                request.TimeZone,
                passwordHasher);

            if (userResult.IsError) return userResult.Errors;

            var newUser = userResult.Value;

            var otpResult = newUser.RequestOtp(identity, OtpPurpose.Registration, codeService, codeHasher);
            if (otpResult.IsError) return otpResult.Errors;

            userRepository.AddUser(newUser);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new RegisterResult(newUser.Id, newUser.PrimaryEmail?.Value, newUser.PhoneNumber);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while registering user {Email} or {Phone}", request.Email, request.PhoneNumber);
            return Error.Failure("Registration.Failed", "An error occurred during registration.");
        }
    }

    static bool IsProvided(string? text) => !string.IsNullOrWhiteSpace(text);
}