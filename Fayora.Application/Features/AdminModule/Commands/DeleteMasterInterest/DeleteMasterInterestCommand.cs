using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteMasterInterest;

public record DeleteMasterInterestCommand(int Id) : ICommand<Result<Success>>;
