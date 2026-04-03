using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone
{
    public class RestoreAccountWithPhoneCommandHandler(
       IUserRepository userRepository,
       IVerificationCodeRepository verificationCodeRepository,
       IUserDeviceManager userDeviceManager,
       IAuthTokenGenerator authTokenGenerator,
       //IUnitOwnerRepository unitOwnerRepository,
       //ITouristRepository touristRepository,
       //ITourGuideRepository tourGuideRepository,
       ICodeHasher codeHasher,
       IUnitOfWork unitOfWork
   ) : IRequestHandler<RestoreAccountWithPhoneCommand, Result<RestoreAccountWithPhoneResult>>
    {
        public async Task<Result<RestoreAccountWithPhoneResult>> Handle(
            RestoreAccountWithPhoneCommand request,
            CancellationToken cancellationToken)
        {
            Language? languageEnum = null;
            if (!Enum.TryParse<Language>(request.DeviceLanguage, true, out var parsedLanguage))
                return AuthErrors.InvalidLanguage;
            languageEnum = parsedLanguage;

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
                languageEnum.Value,
                cancellationToken);

            // 5 - Generate Tokens
            Guid? ownerId = null;
            Guid? touristId = null;
            Guid? tourGuideId = null;

            var roleNames = user.GetRoleNames();

            //if (roleNames.Contains("Owner", StringComparer.OrdinalIgnoreCase))
            //{
            //    var owner = await unitOwnerRepository.GetOwnerByUserIdAsync(user.Id, isReadOnly: true, cancellationToken);
            //    ownerId = owner?.Id;
            //}

            //if (roleNames.Contains("Tourist", StringComparer.OrdinalIgnoreCase))
            //{
            //    var tourist = await touristRepository.GetProfileByUserIdAsync(user.Id, isReadOnly: true, cancellationToken);
            //    touristId = tourist?.Id;
            //}

            //if (roleNames.Contains("TourGuide", StringComparer.OrdinalIgnoreCase))
            //{
            //    var tourGuide = await tourGuideRepository.GetProfileByUserIdAsync(user.Id, isReadOnly: true, cancellationToken);
            //    tourGuideId = tourGuide?.Id;
            //}

            var tokens = await authTokenGenerator.GenerateTokensAsync(
                user,
                request.DeviceId,
                touristId: touristId,
                tourGuideId: tourGuideId,
                ownerId: ownerId,
                cancellationToken);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new RestoreAccountWithPhoneResult(
                user.Id,
                user.FirstName,
                user.LastName,
                request.PhoneNumber,
                user.ProfileImageUrl,
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);
        }
    }
}
