using Fayora.Domain.Entities.ChatModule;
using System;
using System.Collections.Generic;

namespace Fayora.Application.Features.ChatModule.Queries.GetMessages;

public record GetMessagesResult(List<Message> Messages);