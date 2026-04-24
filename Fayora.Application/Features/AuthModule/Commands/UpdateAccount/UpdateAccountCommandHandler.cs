using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Application.Features.AuthModule.Common;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.AuthModule.Commands.UpdateAccount;

public class UpdateAccountCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider) : ICommandHandler<UpdateAccountCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var parsedUserLanguages = new List<UserLanguageProficiency>();

        if (request.UserLanguages is not null && request.UserLanguages.Count != 0)
        {
            foreach (var langDto in request.UserLanguages)
            {
                parsedUserLanguages.Add(new UserLanguageProficiency(langDto.Language, langDto.ProficiencyLevel));
            }
        }

        FileUrl? profileImageUrl = null;
        if (request.ProfileImageUrl is not null)
        {
            var result = FileUrl.Create(request.ProfileImageUrl);
            if (result.IsError) return result.Errors;
            profileImageUrl = result.Value;
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
            request.Gender,
            profileImageUrl,
            request.Description,
            request.NationalityCode,
            request.PreferredLanguage,
            parsedUserLanguages,
            request.TimeZone
        );

        await unitOfWork.CommitChangesAsync(cancellationToken);
        return new Success();
    }
}
