using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Commands.LoginWithApple;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

public class LoginWithAppleCommandHandler(
    IAppleAuthService appleAuthService,
    IUserRepository userRepository,
    IUserIdentityRepository userIdentityRepository,
    IAuthTokenGenerator authTokenGenerator,
    IUserDeviceManager userDeviceManager)
    : ICommandHandler<LoginWithAppleCommand, Result<LoginWithAppleResult>>
{
    public async Task<Result<LoginWithAppleResult>> Handle(LoginWithAppleCommand request, CancellationToken cancellationToken)
    {
        Language? languageEnum = null;
        if (!Enum.TryParse<Language>(request.DeviceLanguage, true, out var parsedLanguage))
            return AuthErrors.InvalidLanguage;
        languageEnum = parsedLanguage;

        var appleUser = await appleAuthService.GetUserInfoAsync(request.IdToken, cancellationToken);
        if (appleUser == null)
            return AuthErrors.InvalidCredentials;

        var identity = await userIdentityRepository.GetIdentityByIdAsync(
            appleUser.SubjectId,
            IdentityProvider.Apple,
            cancellationToken);

        var isFirstLogin = identity == null;

        User? user;
        var options = new UserQueryOptions { IsReadOnly = false};


        if (identity != null)
        {
            user = await userRepository.GetUserByIdAsync(identity.UserId, options, cancellationToken);

            if (user == null)
                return AuthErrors.UserNotFound;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(appleUser.Email))
                return AuthErrors.EmailRequiredFromApple;

            var appleEmail = appleUser.Email;

            user = await userRepository.GetUserByEmailAsync(appleEmail, options, cancellationToken);

            if (user == null)
            {
                user = User.CreateWithSocialLogin(request.FirstName, request.LastName, appleEmail, pictureUrl: null);
                userRepository.AddUser(user);
            }

            var emailValueObject = Email.Create(appleEmail).Value;
            userIdentityRepository.AddIdentity(
                new UserIdentity(
                    user.Id,
                    IdentityProvider.Apple,
                    appleUser.SubjectId,
                    emailValueObject));
        }

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

        var userEmail = user.PrimaryEmail?.Value ?? string.Empty;

        return new LoginWithAppleResult(
            user.Id,
            user.FirstName,
            user.LastName,
            userEmail,
            user.ProfileImageUrl,
            isFirstLogin,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}