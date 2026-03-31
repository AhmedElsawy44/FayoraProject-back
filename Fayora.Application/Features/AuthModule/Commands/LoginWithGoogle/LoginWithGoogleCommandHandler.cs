using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithGoogle;

public class LoginWithGoogleCommandHandler(
    IUserIdentityRepository userIdentityRepository,
    IUserRepository userRepository,
    //IUnitOwnerRepository unitOwnerRepository,
    //ITouristRepository touristRepository,
    //ITourGuideRepository tourGuideRepository,
    IUnitOfWork unitOfWork,
    IGoogleAuthService googleAuthService,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator)
    : IRequestHandler<LoginWithGoogleCommand, Result<LoginWithGoogleResult>>
{
    public async Task<Result<LoginWithGoogleResult>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var googleUser = await googleAuthService.GetUserInfoAsync(request.IdToken, cancellationToken);
        if (googleUser is null) return AuthErrors.InvalidCredentials;

        var existingIdentity = await userIdentityRepository.GetIdentityByIdAsync(
            googleUser.Id,
            IdentityProvider.Google,
            cancellationToken);

        var isFirstLogin = existingIdentity is null;

        var options = new UserQueryOptions { IsReadOnly = false, IncludeRoles = true };
        User? user = null;

        if (existingIdentity is not null)
        {
            user = await userRepository.GetUserByIdAsync(existingIdentity.UserId, options, cancellationToken);
        }

        if (user is null)
        {
            user = await userRepository.GetUserByEmailAsync(googleUser.Email, options, cancellationToken);

            if (user is null)
            {
                user = User.CreateWithSocialLogin(
                    googleUser.Email,
                    googleUser.FirstName,
                    googleUser.LastName,
                    googleUser.PictureUrl);

                user.UpdateRegionalPreferences(
                    request.SimCountryIsoCode,
                    request.TimeZone,
                    request.DeviceLanguage);

                userRepository.AddUser(user);
            }

            var email = Email.Create(googleUser.Email);
            if (email.IsError) return email.Errors;

            var newIdentity = new UserIdentity(
                user.Id,
                IdentityProvider.Google,
                googleUser.Id,
                email.Value);

            userIdentityRepository.AddIdentity(newIdentity);
        }
        else
        {
            var statusCheck = user.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;
        }

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

        return new LoginWithGoogleResult(
            user.Id,
            user.FirstName,
            user.LastName,
            googleUser.Email,
            googleUser.PictureUrl,
            isFirstLogin,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}