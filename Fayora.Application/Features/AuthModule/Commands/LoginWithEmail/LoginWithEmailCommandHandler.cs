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
    //IUnitOwnerRepository unitOwnerRepository,
    //ITouristRepository touristRepository,
    //ITourGuideRepository tourGuideRepository,
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

            Guid? ownerId = null;
            Guid? touristId = null;
            Guid? tourGuideId = null;

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

            return new LoginWithEmailResult(user.Id, request.Email, tokens.AccessToken, tokens.RefreshToken, tokens.ExpiresIn);
        }
    }
}
