using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Contracts.AdminModule.GetUsers;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.UpdateGuideDetails;

public record UpdateGuideDetailsCommand(Guid Id, UpdateGuideDetailsRequest Request) 
    : ICommand<Result<Success>>;
