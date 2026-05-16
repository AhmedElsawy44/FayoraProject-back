using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteLocation
{
    public record DeleteLocationCommand(int LocationId) : ICommand<Result<string>>;
}
