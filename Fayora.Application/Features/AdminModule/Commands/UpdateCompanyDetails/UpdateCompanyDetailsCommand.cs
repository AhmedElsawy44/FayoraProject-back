using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.UpdateCompanyDetails;

public record UpdateCompanyDetailsCommand(Guid Id, UpdateCompanyDetailsRequest Request) 
    : ICommand<Result<Success>>;
