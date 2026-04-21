using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Entities.TourGuide;
using Fayora.Domain.Enums.IdentityModule;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public class CreateTourGuideCommandHandler(
    IUserRepository userRepository,
    ITourGuideRepository tourGuideRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    IJwtService jwtService) : ICommandHandler<CreateTourGuideCommand, Result<CreateTourGuideResult>>
{

    public async Task<Result<CreateTourGuideResult>> Handle(CreateTourGuideCommand request, CancellationToken cancellationToken)
    {
        var parsedUserLanguages = new List<UserLanguageProficiency>();

        if (request.TourGuideLanguages is not null && request.TourGuideLanguages.Count != 0)
        {
            foreach (var langDto in request.TourGuideLanguages)
            {
                parsedUserLanguages.Add(new UserLanguageProficiency(langDto.Language, langDto.ProficiencyLevel));
            }
        }

        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;


        if (user.Roles.HasFlag(Role.TourGuide)) return TourGuideErrors.TourGuideIsAlreadyExist;

        user.AddRole(Role.TourGuide);

        user.UpdateProfile(
            user.FirstName,
            user.LastName,
            request.BirthDate,
            request.Gender,
            request.NationalityCode,
            request.ProfilePictureUrl,
            request.Description,
            request.PreferredLanguage,
            parsedUserLanguages,
            request.TimeZone);

        var tourGuideProfile = TourGuide.Create(
            user.Id,
            request.PricingUnit,
            request.BaseRate,
            request.YearsOfExperience);

        if (tourGuideProfile.IsError) return tourGuideProfile.Errors;

        tourGuideRepository.AddTourGuide(tourGuideProfile.Value);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(request.DeviceId, user);

        return new CreateTourGuideResult
        (
            token,
            jwtService.ExpiresIn
        );
    }
}
