using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteLocation
{
    public record DeleteLocationCommand(int LocationId) : ICommand<Result<string>>;
}
