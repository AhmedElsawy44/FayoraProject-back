using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.LoginWithPhone;

public class LoginWithPhoneCommandHandler(
    IUserRepository userRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : ICommandHandler<LoginWithPhoneCommand, Result<LoginWithPhoneResult>>
{
    public async Task<Result<LoginWithPhoneResult>> Handle(LoginWithPhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByPhoneAsync(
            request.PhoneNumber,
            new UserQueryOptions { IsReadOnly = false },
            cancellationToken);

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

        return new LoginWithPhoneResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.PhoneNumber!.Value,
            user.ProfileImageUrl?.ToString(),
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}