using Fayora.Application.Common.Interfaces.Presistances;
using Fayora.Application.Common.Interfaces.Services;
using Fayora.Application.Features.Auth.Common;
using Fayora.Domain.Common.Interfaces;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IUserRepository;

namespace Fayora.Application.Features.Auth.Commands.LoginWithPhone;

public class LoginWithPhoneCommandHandler(
    IUserRepository userRepository,
    IUserDeviceManager userDeviceManager,
    IAuthTokenGenerator authTokenGenerator,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginWithPhoneCommand, Result<LoginWithPhoneResult>>
{
    public async Task<Result<LoginWithPhoneResult>> Handle(LoginWithPhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByPhoneAsync(
            request.PhoneNumber,
            new UserQueryOptions { IsReadOnly = false, IncludeRoles = true },
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
            request.PhoneNumber,
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}