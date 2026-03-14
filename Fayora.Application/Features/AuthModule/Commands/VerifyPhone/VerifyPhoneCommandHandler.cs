using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.VerifyPhone;

public class VerifyPhoneCommandHandler(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    //IUnitOwnerRepository unitOwnerRepository,
    //ITouristRepository touristRepository,
    //ITourGuideRepository tourGuideRepository,
    ICodeHasher codeHasher,
    IUnitOfWork unitOfWork)
    : IRequestHandler<VerifyPhoneCommand, Result<VerifyPhoneResult>>
{
    public async Task<Result<VerifyPhoneResult>> Handle(VerifyPhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByPhoneAsync(
            request.PhoneNumber,
            new UserQueryOptions { IsReadOnly = false, IncludeRoles = true },
            cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;


        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (user.IsPhoneVerified) return AuthErrors.PhoneIsAlreadyVerified;

        var registerOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id,
            request.PhoneNumber,
            CodePurpose.Registration,
            cancellationToken);

        if (registerOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = registerOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        user.VerifyPhone();
        user.UpdateRegionalPreferences(request.SimCountryIsoCode, request.DeviceLanguage, request.TimeZone);
        user.Login();

        await userDeviceManager.UpsertDeviceAsync(
            user.Id,
            request.DeviceId,
            request.FcmToken,
            request.DeviceLanguage,
            cancellationToken);

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

        return new VerifyPhoneResult(
            user.Id,
            request.PhoneNumber,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}