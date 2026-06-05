using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteCompany;

public record DeleteCompanyCommand(Guid Id) : ICommand<Result<Success>>;
