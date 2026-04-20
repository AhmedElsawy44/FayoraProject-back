using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;
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
    : ICommandHandler<LoginWithGoogleCommand, Result<LoginWithGoogleResult>>
{
    public async Task<Result<LoginWithGoogleResult>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        Language? languageEnum = null;
        if (!Enum.TryParse<Language>(request.DeviceLanguage, true, out var parsedLanguage))
            return AuthErrors.InvalidLanguage;
        languageEnum = parsedLanguage;

        var googleUser = await googleAuthService.GetUserInfoAsync(request.IdToken, cancellationToken);
        if (googleUser is null) return AuthErrors.InvalidCredentials;

        var existingIdentity = await userIdentityRepository.GetIdentityByIdAsync(
            googleUser.Id,
            IdentityProvider.Google,
            cancellationToken);

        var isFirstLogin = existingIdentity is null;

        var options = new UserQueryOptions { IsReadOnly = false };
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
                    googleUser.FirstName,
                    googleUser.LastName,
                    googleUser.Email,
                    googleUser.PictureUrl);

                user.UpdateRegionalPreferences(
                    request.SimCountryIsoCode,
                    languageEnum.Value,
                    request.TimeZone);

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
            languageEnum.Value,
            cancellationToken);

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
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