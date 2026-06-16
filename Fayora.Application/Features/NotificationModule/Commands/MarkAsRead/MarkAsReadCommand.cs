using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;
using System;

namespace Fayora.Application.Features.NotificationModule.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid NotificationId) : ICommand<Result<Success>>;
