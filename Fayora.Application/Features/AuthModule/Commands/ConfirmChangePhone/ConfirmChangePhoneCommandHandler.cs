using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;

public class ConfirmChangePhoneCommandHandlerIUserRepository(
    IUserRepository userRepository,
    IVerificationCodeRepository verificationCodeRepository,
    //IUnitOwnerRepository unitOwnerRepository,
    //ITouristRepository touristRepository,
    //ITourGuideRepository tourGuideRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    ICodeHasher codeHasher,
    IJwtService jwtService)
    : IRequestHandler<ConfirmChangePhoneCommand, Result<ConfirmChangePhoneResult>>
{
    public async Task<Result<ConfirmChangePhoneResult>> Handle(ConfirmChangePhoneCommand request, CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId,
        new UserQueryOptions
        {
            IsReadOnly = false,
            IncludeVerificationCodes = true,
            IncludeRoles = true
        },
        cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        var changePhoneOtp = await verificationCodeRepository.GetUserCodeAsync(
            user.Id, request.NewPhoneNumber, CodePurpose.ChangePhoneNumber, cancellationToken);

        if (changePhoneOtp is null) return AuthErrors.InvalidVerificationCode;

        var verifyResult = changePhoneOtp.Use(request.Code, codeHasher);
        if (verifyResult.IsError)
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return verifyResult.Errors;
        }

        user.ChangePhoneNumber(request.NewPhoneNumber);

        await unitOfWork.CommitChangesAsync(cancellationToken);

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

        var token = jwtService.GenerateToken(
            request.DeviceId,
            user,
            touristId,
            tourGuideId,
            ownerId);

        return new ConfirmChangePhoneResult(user.Id, request.NewPhoneNumber, token, jwtService.ExpiresIn);
    }
}
