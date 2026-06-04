using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

public record AgentDecision(
    [property: JsonPropertyName("action")] string Action,                     // "search" | "ask" | "reply" | "off_topic"
    [property: JsonPropertyName("question")] string? Question,                  // السؤال لو action=ask
    [property: JsonPropertyName("entities")] List<string>? Entities,            // ["housing", "guide", "place"]
    [property: JsonPropertyName("search_queries")] Dictionary<string, string>? SearchQueries, // {"housing": "فندق هادئ رومانسي"}
    [property: JsonPropertyName("filters")] SearchFilters? Filters,
    [property: JsonPropertyName("reasoning")] string? Reasoning
);

public record SearchFilters(
    [property: JsonPropertyName("guests")] int? Guests,
    [property: JsonPropertyName("budget_max")] decimal? BudgetMax,
    [property: JsonPropertyName("area")] string? Area
);

public record ChatbotCard(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("itemType")] string ItemType,      // "housing_unit" | "guide_package" | "place"
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("subtitle")] string Subtitle,
    [property: JsonPropertyName("priceLabel")] string? PriceLabel,
    [property: JsonPropertyName("rating")] double Rating,
    [property: JsonPropertyName("imageUrl")] string? ImageUrl,
    [property: JsonPropertyName("mapLink")] string? MapLink
);

public record ChatbotFinalResponse(
    [property: JsonPropertyName("sessionId")] Guid SessionId,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("cards")] List<ChatbotCard> Cards,
    [property: JsonPropertyName("suggestions")] List<string> Suggestions,
    [property: JsonPropertyName("conversationId")] string? ConversationId
);

public record ChatMessageDto(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content
);
