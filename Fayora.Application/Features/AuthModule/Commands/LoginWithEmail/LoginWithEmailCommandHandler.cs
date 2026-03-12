using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithEmail
{
    public class LoginWithEmailCommandHandler(
    IUserRepository userRepository,
    IUserDeviceManager userDeviceManager,
    IUnitOfWork unitOfWork,
    IAuthTokenGenerator authTokenGenerator,
    IPasswordHasher passwordHasher) : IRequestHandler<LoginWithEmailCommand, Result<LoginWithEmailResult>>
    {
        public async Task<Result<LoginWithEmailResult>> Handle(LoginWithEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByEmailAsync(request.Email, new UserQueryOptions { IsReadOnly = false, IncludeRoles = true }, cancellationToken);

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

            await userDeviceManager.UpsertDeviceAsync(user.Id, request.DeviceId, request.FcmToken, request.DeviceLanguage, cancellationToken);

            var tokens = await authTokenGenerator.GenerateTokensAsync(user, request.DeviceId, cancellationToken);

            await unitOfWork.CommitChangesAsync(cancellationToken);

            return new LoginWithEmailResult(user.Id, request.Email, tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresIn);
        }
    }
}
