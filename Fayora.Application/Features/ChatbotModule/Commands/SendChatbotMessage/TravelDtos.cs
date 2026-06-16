namespace Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;

public record TravelSearchParametersDto(
    string? Destination,
    decimal? Budget,
    int? Guests,
    string? TripType
);

public record TravelOptionDto(
    Guid Id,
    string Title,
    string Description,
    string Destination,
    decimal Price,
    double Rating,
    string OptionType
);

public record ChatMessageDto(
    string Role,
    string Content
);
