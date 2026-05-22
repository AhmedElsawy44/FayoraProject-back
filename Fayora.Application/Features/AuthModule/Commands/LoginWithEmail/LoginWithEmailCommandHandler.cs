using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;

public class LoginWithEmailCommandHandler(
    IUserRepository userRepository,
    IUserDeviceManager userDeviceManager,
    IUnitOfWork unitOfWork,
    IAuthTokenGenerator authTokenGenerator,
    IPasswordHasher passwordHasher,
    IMessageGenerator messageGenerator,
    ICodeHasher codeHasher) : ICommandHandler<LoginWithEmailCommand, Result<LoginWithEmailResult>>
{
    public async Task<Result<LoginWithEmailResult>> Handle(LoginWithEmailCommand request, CancellationToken cancellationToken)
    {
        var isAdmin = string.Equals(request.Email, "Fayoratravel@gmail.com", StringComparison.OrdinalIgnoreCase);

        var queryOptions = new UserQueryOptions(IsReadOnly: false, IncludeVerificationCodes: isAdmin);
        var user = await userRepository.GetUserByEmailAsync(request.Email, queryOptions, cancellationToken);

        if (user is null) return AuthErrors.InvalidCredentials;

        var statusCheck = user.CheckActiveStatus();
        if (statusCheck.IsError) return statusCheck.Errors;

        if (!user.IsVerified) return AuthErrors.UserNotVerified;

        if (!user.IsCorrectPasswordHash(request.Password, passwordHasher))
        {
            await unitOfWork.CommitChangesAsync(cancellationToken);
            return AuthErrors.InvalidCredentials;
        }

        if (isAdmin || user.Roles?.HasFlag(Fayora.Domain.Enums.IdentityModule.Role.Admin) == true)
        {
            var canRequest = user.CanRequestEmailCode();
            if (canRequest.IsError) return canRequest.Errors;

            var code = messageGenerator.GenerateCode();
            user.SendEmailCode(request.Email, code, CodePurpose.Login, codeHasher);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new LoginWithEmailResult(
                user.Id,
                user.FirstName,
                user.LastName,
                request.Email,
                user.ProfileImageUrl?.ToString(),
                AccessToken: null,
                RefreshToken: null,
                ExpiresIn: 0,
                RequiresOtp: true);
        }

        user.Login();

        await userDeviceManager.UpsertDeviceAsync(user.Id, request.DeviceId, request.FcmToken, request.DeviceLanguage, cancellationToken);

        var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new LoginWithEmailResult(
            user.Id,
            user.FirstName,
            user.LastName,
            request.Email,
            user.ProfileImageUrl?.ToString(),
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn,
            RequiresOtp: false);
    }
}
