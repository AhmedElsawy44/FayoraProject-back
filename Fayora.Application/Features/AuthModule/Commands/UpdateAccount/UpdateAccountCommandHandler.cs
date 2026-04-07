using Fayora.Application.Common.Interfaces.Presistances.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.IdentityModule;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Presistances.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public class UpdateAccountCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider) : IRequestHandler<UpdateAccountCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        Gender? genderEnum = null;
        if (!string.IsNullOrWhiteSpace(request.Gender))
        {
            if (!Enum.TryParse<Gender>(request.Gender, true, out var parsedGender))
                return AuthErrors.InvalidGender;
            genderEnum = parsedGender;
        }

        Language? languageEnum = null;
        if (!string.IsNullOrWhiteSpace(request.PreferredLanguage))
        {
            if (!Enum.TryParse<Language>(request.PreferredLanguage, true, out var parsedLanguage))
                return AuthErrors.InvalidLanguage;
            languageEnum = parsedLanguage;
        }

        var parsedUserLanguages = new List<UserLanguageProficiency>();

        if (request.UserLanguages is not null && request.UserLanguages.Count != 0)
        {
            foreach (var langDto in request.UserLanguages)
            {
                if (!Enum.TryParse<Language>(langDto.Language, true, out var lang))
                    return AuthErrors.InvalidLanguage;

                if (langDto.ProficiencyLevel < 0 || langDto.ProficiencyLevel > 1)
                    return AuthErrors.InvalidLanguageLevel;

                parsedUserLanguages.Add(new UserLanguageProficiency(lang, langDto.ProficiencyLevel));
            }
        }

        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

        if (user is null) return AuthErrors.UserNotFound;

        var check = user.CheckActiveStatus();

        if (check.IsError) return check.Errors;

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            genderEnum,
            request.NationalityCode,
            request.ProfileImageUrl,
            request.Description,
            languageEnum,
            parsedUserLanguages,
            request.TimeZone
        );

        await unitOfWork.CommitChangesAsync(cancellationToken);
        return new Success();
    }
}
