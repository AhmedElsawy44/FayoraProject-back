using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithFacebook;

public class LoginWithFacebookCommandHandler(
    IUserRepository userRepository,
    IUserIdentityRepository userIdentityRepository,
    //IUnitOwnerRepository unitOwnerRepository,
    //ITouristRepository touristRepository,
    //ITourGuideRepository tourGuideRepository,
    IFacebookAuthService facebookAuthService,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    IUnitOfWork unitOfWork
) : IRequestHandler<LoginWithFacebookCommand, Result<LoginWithFacebookResult>>
{
    public async Task<Result<LoginWithFacebookResult>> Handle(LoginWithFacebookCommand request, CancellationToken cancellationToken)
    {
        Language? languageEnum = null;
        if (!Enum.TryParse<Language>(request.DeviceLanguage, true, out var parsedLanguage))
            return AuthErrors.InvalidLanguage;
        languageEnum = parsedLanguage;

        var facebookUser = await facebookAuthService.GetUserInfoAsync(request.AccessToken, cancellationToken);
        if (facebookUser is null)
            return AuthErrors.InvalidCredentials;

        var existingIdentity = await userIdentityRepository.GetIdentityByIdAsync(
            facebookUser.Id,
            IdentityProvider.Facebook,
            cancellationToken);

        bool isFirstLogin = existingIdentity is null;

        User? user = null;

        if (existingIdentity is not null)
        {
            user = await userRepository.GetUserByIdAsync(
                existingIdentity.UserId,
                new UserQueryOptions { IsReadOnly = false, IncludeRoles = true },
                cancellationToken);
        }

        if (user is null)
        {
            user = User.CreateWithSocialLogin(
                facebookUser.Name,
                facebookUser.Email,
                facebookUser.PictureUrl);

            user.UpdateRegionalPreferences(
                request.SimCountryIsoCode,
                languageEnum.Value,
                request.TimeZone);

            userRepository.AddUser(user);

            var email = Email.Create(facebookUser.Email);

            var identity = new UserIdentity(
                user.Id,
                IdentityProvider.Facebook,
                facebookUser.Id,
                email.Value);

            userIdentityRepository.AddIdentity(identity);
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
            languageEnum.Value,
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

        return new LoginWithFacebookResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.PrimaryEmail?.Value ?? string.Empty,
            user.ProfileImageUrl,
            isFirstLogin,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}