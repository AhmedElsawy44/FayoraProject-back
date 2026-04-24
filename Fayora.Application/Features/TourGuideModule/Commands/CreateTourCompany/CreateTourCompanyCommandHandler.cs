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

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateTourCompany;


public class CreateTourCompanyCommandHandler(
    IUserRepository userRepository,
    ITourCompanyRepository tourCompanyRepository,
    IUnitOfWork unitOfWork,
    IClientContextProvider clientContextProvider,
    IAuthTokenGenerator authTokenGenerator)
    : ICommandHandler<CreateTourCompanyCommand, Result<CreateTourCompanyResult>>
{
    public async Task<Result<CreateTourCompanyResult>> Handle(
        CreateTourCompanyCommand command,
        CancellationToken cancellationToken)
    {
        var image = FileUrl.Create(command.ProfilePictureUrl);
        if (image.IsError)
            return image.Errors;

        var userId = clientContextProvider.GetContext().UserId;

        var user = await userRepository.GetUserByIdAsync(userId, new UserQueryOptions { IsReadOnly = false }, cancellationToken);

        if (user is null)
            return AuthErrors.UserNotFound;

        if (user.Roles.HasFlag(Role.TourCompany)) return TourCompanyErrors.TourCompanyIsAlreadyExist;


        if (await tourCompanyRepository.TourCompanyExistAsync(userId, cancellationToken)) return TourCompanyErrors.TourCompanyIsAlreadyExist;

        user.AddRole(Role.TourCompany);

        user.UpdateProfile(
            user.FirstName,
            user.LastName,
            null,
            null,
            image.Value,
            command.Description,
            null,
            null,
            [],
            user.TimeZone
            );

        var companyResult = TourCompany.Create(
            userId,
            command.CompanyName);

        if (companyResult.IsError)
            return companyResult.Errors;

        var company = companyResult.Value;

        tourCompanyRepository.AddTourCompany(company);

        var token = await authTokenGenerator.GenerateTokensAsync(
            user,
            command.DeviceId,
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return new CreateTourCompanyResult
        (
            user.Id,
            company.CompanyName,
            company.Status.ToString(),
            token.AccessToken,
            token.RefreshToken,
            token.ExpiresIn
        );
    }
}
