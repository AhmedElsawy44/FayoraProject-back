using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Commands.LoginWithApple;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

public class LoginWithAppleCommandHandler(
    IAppleAuthService appleAuthService,
    IUserRepository userRepository,
    IUserIdentityRepository userIdentityRepository,
    IAuthTokenGenerator authTokenGenerator,
    IUserDeviceManager userDeviceManager)
    : IRequestHandler<LoginWithAppleCommand, Result<LoginWithAppleResult>>
{
    public async Task<Result<LoginWithAppleResult>> Handle(LoginWithAppleCommand request, CancellationToken cancellationToken)
    {
        var appleUser = await appleAuthService.GetUserInfoAsync(request.AccessToken, cancellationToken);
        if (appleUser == null)
            return AuthErrors.InvalidCredentials;

        var identity = await userIdentityRepository.GetIdentityByIdAsync(
            appleUser.SubjectId,
            IdentityProvider.Apple,
            cancellationToken);

        User? user;
        var options = new UserQueryOptions { IsReadOnly = false, IncludeRoles = true };

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

            user = await userRepository.GetUserByEmailAsync(appleUser.Email, options, cancellationToken);

            if (user == null)
            {
                user = User.CreateWithSocialLogin(appleUser.Email);
                userRepository.AddUser(user);
            }

            var emailValueObject = Email.Create(appleUser.Email).Value;

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
            request.DeviceLanguage,
            cancellationToken);

        var authResult = await authTokenGenerator.GenerateTokensAsync(user, request.DeviceId, cancellationToken);

        var userEmail = user.PrimaryEmail?.Value ?? string.Empty;

        return new LoginWithAppleResult(
            user.Id,
            userEmail,
            authResult.AccessToken,
            authResult.RefreshToken);
    }
}