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

                if (await userRepository.IsIdentityExistAsync(request.Email!, IdentityType.Email, cancellationToken, AccountStatus.Verified))
                    return UserErrors.EmailAlreadyExists;
            }

            if (IsProvided(request.PhoneNumber))
            {
                if (await bannedItemRepository.IsBannedAsync(BanType.PhoneNumber, request.PhoneNumber!, cancellationToken))
                    return UserErrors.PhoneBanned;

                if (await userRepository.IsIdentityExistAsync(request.PhoneNumber!, IdentityType.Phone, cancellationToken, AccountStatus.Verified))
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

            var target = IsProvided(request.Email) ? request.Email : request.PhoneNumber;
            var vCode = codeService.GenerateCode();
            var vCodeHash = codeHasher.HashCode(vCode);

            var otpResult = user.RequestOtp(target!, vCode, OtpPurpose.Registration, vCodeHash);
            if (otpResult.IsError)
            {
                return otpResult.Errors;
            }

            userRepository.AddUser(user);
            await unitOfWork.CommitChangesAsync(cancellationToken);


            return new RegisterResult(user.Id, user.PrimaryEmail?.Value, user.PhoneNumber);
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