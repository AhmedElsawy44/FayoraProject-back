using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail
{
    public class LoginWithEmailCommandHandler(
    IUserRepository userRepository,
    IUserDeviceManager userDeviceManager,
    IUnitOfWork unitOfWork,
    IAuthTokenGenerator authTokenGenerator,
    IPasswordHasher passwordHasher) : ICommandHandler<LoginWithEmailCommand, Result<LoginWithEmailResult>>
    {
        public async Task<Result<LoginWithEmailResult>> Handle(LoginWithEmailCommand request, CancellationToken cancellationToken)
        {
            Language? languageEnum = null;
            if (!Enum.TryParse<Language>(request.DeviceLanguage, true, out var parsedLanguage))
                return AuthErrors.InvalidLanguage;
            languageEnum = parsedLanguage;

            var user = await userRepository.GetUserByEmailAsync(request.Email, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

            if (user is null) return AuthErrors.InvalidCredentials;

            var statusCheck = user.CheckActiveStatus();
            if (statusCheck.IsError) return statusCheck.Errors;

            if (!user.IsVerified) return AuthErrors.UserNotVerified;

            if (!user.IsCorrectPasswordHash(request.Password, passwordHasher))
            {
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return AuthErrors.InvalidCredentials;
            }

            user.Login();

            await userDeviceManager.UpsertDeviceAsync(user.Id, request.DeviceId, request.FcmToken, languageEnum.Value, cancellationToken);

            var tokens = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new LoginWithEmailResult(
                user.Id, user.FirstName,
                user.LastName,
                request.Email,
                user.ProfileImageUrl,
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);
        }
    }
}
