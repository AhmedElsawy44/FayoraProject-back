using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using System;

namespace Fayora.Application.Features.BookingModule.Commands.CreateProviderPayout;

public record CreateProviderPayoutCommand() : ICommand<Result<Guid>>;
