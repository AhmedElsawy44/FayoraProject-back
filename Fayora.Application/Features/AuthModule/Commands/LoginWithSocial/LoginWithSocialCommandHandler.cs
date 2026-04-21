using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.ValueObjects;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithSocial;

public class LoginWithSocialCommandHandler(
    ISocialAuthService socialAuthService,
    IUserRepository userRepository,
    IUserIdentityRepository userIdentityRepository,
    IAuthTokenGenerator authTokenGenerator,
    IUserDeviceManager userDeviceManager,
    IUnitOfWork unitOfWork)
    : ICommandHandler<LoginWithSocialCommand, Result<LoginWithSocialResult>>
{
    public async Task<Result<LoginWithSocialResult>> Handle(LoginWithSocialCommand request, CancellationToken cancellationToken)
    {
        var socialUser = await socialAuthService.GetUserInfoAsync(request.Token, request.IdentityProvider, cancellationToken);
        if (socialUser is null)
            return AuthErrors.InvalidCredentials;

        var identity = await userIdentityRepository.GetIdentityByIdAsync(
            socialUser.SubjectId,
            request.IdentityProvider,
            cancellationToken);

        var isFirstLogin = identity is null;
        var options = new UserQueryOptions { IsReadOnly = false };

        User? user;

        if (identity is not null)
        {
            user = await userRepository.GetUserByIdAsync(identity.UserId, options, cancellationToken);
            if (user is null)
                return AuthErrors.UserNotFound;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(socialUser.Email))
                return AuthErrors.EmailRequiredFromIdentityProvider;

            var emailResult = Email.Create(socialUser.Email);
            if (emailResult.IsError)
                return emailResult.Errors;

            var email = emailResult.Value;

            user = await userRepository.GetUserByEmailAsync(email.Value, options, cancellationToken)
                   ?? CreateAndAddUser(request, socialUser.Email, socialUser.FirstName, socialUser.LastName, socialUser.PictureUrl);

            userIdentityRepository.AddIdentity(new UserIdentity(
                user.Id,
                request.IdentityProvider,
                socialUser.SubjectId,
                email));
        }

        user.Login();

        await userDeviceManager.UpsertDeviceAsync(
            user.Id,
            request.DeviceId,
            request.FcmToken,
            request.DeviceLanguage,
            cancellationToken);

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);


        await unitOfWork.CommitChangesAsync(cancellationToken);

        var userEmail = user.PrimaryEmail?.Value ?? string.Empty;

        return new LoginWithSocialResult(
            user.Id,
            user.FirstName,
            user.LastName,
            userEmail,
            user.ProfileImageUrl?.Value,
            isFirstLogin,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }

    private User CreateAndAddUser(LoginWithSocialCommand request, string email, string? providerFirstName, string? providerLastName, string? pictureUrl)
    {
        var firstName = providerFirstName ?? request.FirstName ?? "User";
        var lastName = providerLastName ?? request.LastName ?? string.Empty;

        var user = User.CreateWithSocialLogin(firstName, lastName, email, pictureUrl);
        userRepository.AddUser(user);
        return user;
    }
}