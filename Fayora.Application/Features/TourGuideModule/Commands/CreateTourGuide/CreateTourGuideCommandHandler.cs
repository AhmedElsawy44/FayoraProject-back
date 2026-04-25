using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.ValueObjects;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public class CreateTourGuideCommandHandler(
    IUserRepository userRepository,
    ITourGuideRepository tourGuideRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    IAuthTokenGenerator authTokenGenerator) : ICommandHandler<CreateTourGuideCommand, Result<CreateTourGuideResult>>
{

    public async Task<Result<CreateTourGuideResult>> Handle(CreateTourGuideCommand request, CancellationToken cancellationToken)
    {
        var professionalLicenseUrl = FileUrl.Create(request.ProfessionalLicenseUrl);
        if (professionalLicenseUrl.IsError) return professionalLicenseUrl.Errors;

        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;


        if (user.Roles is not null) return TourGuideErrors.CannotBeTourGuide;

        if (await tourGuideRepository.TourGuideExistAsync(userId, cancellationToken)) return TourGuideErrors.TourGuideIsAlreadyExist;

        user.AddRole(Role.TourGuide);

        var tourGuide = new TourGuide(userId, professionalLicenseUrl.Value);

        tourGuideRepository.AddTourGuide(tourGuide);


        var token = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateTourGuideResult
        (
            tourGuide.UserId,
            user.FirstName,
            user.LastName,
            tourGuide.Status.ToString(),
            token.AccessToken,
            token.RefreshToken,
            token.ExpiresIn
        );
    }
}
