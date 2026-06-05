using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.DeleteTourPackage;

public record DeleteTourPackageCommand(Guid Id) : ICommand<Result<Success>>;
