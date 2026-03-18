using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Interfaces.IdentityModule;
using Fayora.Domain.Common.Results;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.ChangePhone
{
    public class ChangePhoneCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IClientContextProvider clientContextProvider,
        IPasswordHasher passwordHasher,
        IAuthTokenGenerator authTokenGenerator) : IRequestHandler<ChangePhoneCommand, Result<ChangePhoneResult>>
    {

        public async Task<Result<ChangePhoneResult>> Handle(ChangePhoneCommand request, CancellationToken cancellationToken)
        {
            var userId = clientContextProvider.GetContext().UserId;

            var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IncludeRoles = true, IsReadOnly = false }, cancellationToken);

            if (user is null) return AuthErrors.UserNotFound;

            if (!user.IsCorrectPasswordHash(request.Password, passwordHasher)) return AuthErrors.InvalidPassword;

            if (await userRepository.IsPhoneNumberExistsAsync(request.PhoneNumber, cancellationToken)) return AuthErrors.PhoneNumberAlreadyExists;

            user.ChangePhoneNumber(request.PhoneNumber);

            Guid? ownerId = null;
            Guid? touristId = null;
            Guid? tourGuideId = null;

            var roleNames = user.GetRoleNames();

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


            return new ChangePhoneResult(
                user.Id,
                request.PhoneNumber,
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.ExpiresIn);
        }
    }
}
