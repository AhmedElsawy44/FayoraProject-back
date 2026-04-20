using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.TourGuideModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Application.Features.TourGuideModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Entities.TourGuide;
using Fayora.Domain.Enums.IdentityModule;
using Fayora.Domain.Enums.TourGuideModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourGuide;

public class CreateTourGuideCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ITourGuideRepository tourGuideRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    IJwtService jwtService) : IRequestHandler<CreateTourGuideCommand, Result<CreateTourGuideResult>>
{

    public async Task<Result<CreateTourGuideResult>> Handle(CreateTourGuideCommand request, CancellationToken cancellationToken)
    {
        if (Enum.TryParse<PricingUnit>(request.PricingUnit, true, out var pricingUnit) == false)
        {
            return TourGuideErrors.InvalidPricingUnit;
        }

        if (Enum.TryParse<Language>(request.PreferredLanguage, true, out var preferredLanguage) == false)
        {
            return AuthErrors.InvalidLanguage;
        }

        var parsedUserLanguages = new List<UserLanguageProficiency>();

        if (request.TourGuideLanguages is not null && request.TourGuideLanguages.Count != 0)
        {
            foreach (var langDto in request.TourGuideLanguages)
            {
                if (!Enum.TryParse<Language>(langDto.Language, true, out var lang))
                    return AuthErrors.InvalidLanguage;

                parsedUserLanguages.Add(new UserLanguageProficiency(lang, langDto.ProficiencyLevel));
            }
        }

        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IncludeRoles = true, IsReadOnly = false }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        if (user.Roles.Count != 0) return TourGuideErrors.HasRole;

        var role = await roleRepository.GetRoleByNameAsync("TourGuide", cancellationToken);

        if (role is null) return Error.Validation("TourGuide.RoleNotFound", "TourGuide role does not exist.");

        user.AddRole(role);

        user.UpdateProfile(
            user.FirstName,
            user.LastName,
            user.BirthDate,
            user.Gender,
            user.NationalityCode,
            request.ProfilePictureUrl ?? user.ProfileImageUrl,
            request.Description ?? user.Description,
            preferredLanguage,
            parsedUserLanguages,
            user.TimeZone
            );

        var tourGuideProfile = TourGuide.Create(
            user.Id,
            pricingUnit,
            request.BaseRate,
            request.YearsOfExperience,
            request.LicenseNumber,
            request.LicenseExpiryDate,
            request.CurrencyCode
            );

        if (tourGuideProfile.IsError) return tourGuideProfile.Errors;

        tourGuideRepository.AddTourGuide(tourGuideProfile.Value);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        var token = jwtService.GenerateToken(request.DeviceId, user, null, tourGuideProfile.Value.Id, null);

        return new CreateTourGuideResult
        (
            token,
            jwtService.ExpiresIn
        );
    }
}
