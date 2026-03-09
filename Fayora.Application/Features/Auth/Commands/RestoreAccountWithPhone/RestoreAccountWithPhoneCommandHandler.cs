using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthServices;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Shared.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.RestoreAccountWithPhone
{
    public class RestoreAccountWithPhoneCommandHandler(
       IUserRepository userRepository,
       IVerificationCodeRepository verificationCodeRepository,
       IUserDeviceManager userDeviceManager,
       IAuthTokenGenerator authTokenGenerator,
       ICodeHasher codeHasher,
       IUnitOfWork unitOfWork
   ) : IRequestHandler<RestoreAccountWithPhoneCommand, Result<RestoreAccountWithPhoneResult>>
    {
        public async Task<Result<RestoreAccountWithPhoneResult>> Handle(
            RestoreAccountWithPhoneCommand request,
            CancellationToken cancellationToken)
        {
            // 1 - Get the user
            var user = await userRepository.GetUserByPhoneAsync(
                request.PhoneNumber,
                new UserQueryOptions
                {
                    IsReadOnly = false,
                    IncludeVerificationCodes = true,
                    IncludeRoles = true,
                    UserStatus = UserStatus.Deleted
                },
                cancellationToken);

            if (user is null || !user.IsDeleted)
                return AuthErrors.UserNotFound;

            // 2 - Check OTP
            var otp = await verificationCodeRepository.GetUserCodeAsync(
                user.Id,
                request.PhoneNumber,
                CodePurpose.ReactivateAccount,
                cancellationToken);

            if (otp is null)
                return AuthErrors.InvalidVerificationCode;

            var verifyResult = otp.Use(request.Code, codeHasher);
            if (verifyResult.IsError)
            {
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return verifyResult.Errors;
            }

            // 3 - Restore
            user.Restore();
            user.Login();

            // 4 - Upsert Device
            await userDeviceManager.UpsertDeviceAsync(
                user.Id,
                request.DeviceId,
                request.FcmToken,
                request.DeviceLanguage,
                cancellationToken);

            // 5 - Generate Tokens
            var tokens = await authTokenGenerator.GenerateTokensAsync(
                user,
                request.DeviceId,
                cancellationToken);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new RestoreAccountWithPhoneResult(
                user.Id,
                request.PhoneNumber,
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);
        }
    }
}
