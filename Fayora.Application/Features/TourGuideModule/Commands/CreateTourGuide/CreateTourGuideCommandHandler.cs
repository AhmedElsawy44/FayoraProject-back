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
        var parsedUserLanguages = new List<UserLanguageProficiency>();

        if (request.TourGuideLanguages is not null && request.TourGuideLanguages.Count != 0)
        {
            foreach (var langDto in request.TourGuideLanguages)
            {
                parsedUserLanguages.Add(new UserLanguageProficiency(langDto.Language, langDto.ProficiencyLevel));
            }
        }

        var imageUrl = FileUrl.Create(request.ProfilePictureUrl);
        if (imageUrl.IsError) return imageUrl.Errors;

        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;


        if (user.Roles.HasFlag(Role.TourGuide)) return TourGuideErrors.TourGuideIsAlreadyExist;

        if (await tourGuideRepository.TourGuideExistAsync(userId, cancellationToken)) return TourGuideErrors.TourGuideIsAlreadyExist;

        user.AddRole(Role.TourGuide);

        user.UpdateProfile(
            user.FirstName,
            user.LastName,
            request.BirthDate,
            request.Gender,
            imageUrl.Value,
            request.Description,
            request.NationalityCode,
            request.PreferredLanguage,
            parsedUserLanguages,
            request.TimeZone);

        var tourGuide = TourGuide.Create(
            user.Id,
            request.PricingUnit,
            request.BaseRate,
            request.YearsOfExperience);

        if (tourGuide.IsError) return tourGuide.Errors;

        tourGuideRepository.AddTourGuide(tourGuide.Value);


        var token = await authTokenGenerator.GenerateTokensAsync(
            user,
            request.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateTourGuideResult
        (
            tourGuide.Value.UserId,
            user.FirstName,
            user.LastName,
            tourGuide.Value.Status.ToString(),
            token.AccessToken,
            token.RefreshToken,
            token.ExpiresIn
        );
    }
}
