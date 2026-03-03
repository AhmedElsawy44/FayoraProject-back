using Fayora.Application.Common.Interfaces.Presistance;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
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
    IVerificationCodeRepository verificationCodeRepository,
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

            if (IsProvided(request.Email))
            {
                if (await bannedItemRepository.IsBannedAsync(BanType.Email, request.Email!, cancellationToken))
                    return UserErrors.EmailBanned;

                var existUser = await userRepository.GetUserByIdentity(request.Email!, null, cancellationToken, AccountStatus.All);

                if (existUser != null)
                {
                    if (existUser.IsEmailVerified)
                        return UserErrors.EmailAlreadyExists;

                    var acountOtpResult = existUser.RequestOtp(request.Email!, null, OtpPurpose.Registration, codeService, codeHasher);
                    if (acountOtpResult.IsError) return acountOtpResult.Errors;

                    await unitOfWork.CommitChangesAsync(cancellationToken);

                    return new RegisterResult(existUser.Id, existUser.PrimaryEmail?.Value, existUser.PhoneNumber);
                }
            }

            if (IsProvided(request.PhoneNumber))
            {
                if (await bannedItemRepository.IsBannedAsync(BanType.PhoneNumber, request.PhoneNumber!, cancellationToken))
                    return UserErrors.PhoneBanned;

                var existUser = await userRepository.GetUserByIdentity(request.PhoneNumber!, request.SimCountryIsoCode, cancellationToken, AccountStatus.All);

                if (existUser != null)
                {
                    if (existUser.IsPhoneVerified)
                        return UserErrors.PhoneAlreadyExists;

                    var acountOtpResult = existUser.RequestOtp(request.PhoneNumber!, request.SimCountryIsoCode, OtpPurpose.Registration, codeService, codeHasher);
                    if (acountOtpResult.IsError) return acountOtpResult.Errors;

                    await unitOfWork.CommitChangesAsync(cancellationToken);

                    return new RegisterResult(existUser.Id, existUser.PrimaryEmail?.Value, existUser.PhoneNumber);
                }
            }

            var passwordHashResult = passwordHasher.HashPassword(request.Password);
            if (passwordHashResult.IsError) return UserErrors.InvalidPassword;

            var userResult = User.Create(request.Email, request.PhoneNumber, passwordHashResult.Value, request.SimCountryIsoCode, request.DeviceLanguage, request.TimeZone);
            if (userResult.IsError) return userResult.Errors;

            var user = userResult.Value;
            var target = IsProvided(request.Email) ? request.Email : request.PhoneNumber;

            var otpResult = user.RequestOtp(target!, request.SimCountryIsoCode, OtpPurpose.Registration, codeService, codeHasher);
            if (otpResult.IsError) return otpResult.Errors;

            userRepository.AddUser(user);
            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new RegisterResult(user.Id, user.PrimaryEmail?.Value, user.PhoneNumber);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while registering user {Email} or {Phone}", request.Email, request.PhoneNumber);
            return Error.Failure("Registration.Failed", "An error occurred during registration.");
        }
    }

    static bool IsProvided(string? text) => !string.IsNullOrWhiteSpace(text);
}