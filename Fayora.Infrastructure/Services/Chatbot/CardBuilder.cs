using System;
using Fayora.Application.Features.ChatbotModule.Commands.SendChatbotMessage;
using Fayora.Domain.Entities.AccommodationModule;
using Fayora.Domain.Entities.GuideModule;
using Fayora.Domain.Entities.SharedModule;

namespace Fayora.Infrastructure.Services.Chatbot;

public static class CardBuilder
{
    public static ChatbotCard FromHousingUnit(HousingUnit item)
    {
        return new ChatbotCard(
            Id: item.Id.ToString(),
            ItemType: "housing_unit",
            Title: item.Title,
            Subtitle: $"{item.AddressDetails} — ⭐{item.Rating:F1}",
            PriceLabel: $"{item.PricePerNight:F0} جنيه/ليلة",
            Rating: (double)item.Rating,
            ImageUrl: item.MainImageUrl?.Value,
            MapLink: item.Coordinates != null ? $"https://www.google.com/maps/search/?api=1&query={item.Coordinates.Latitude},{item.Coordinates.Longitude}" : null
        );
    }

    public static ChatbotCard FromGuidePackage(GuidePackage item)
    {
        return new ChatbotCard(
            Id: item.Id.ToString(),
            ItemType: "guide_package",
            Title: item.Title,
            Subtitle: $"{item.DurationHours} ساعة — {item.TourTypes}",
            PriceLabel: $"{item.AdultPrice:F0} جنيه/شخص",
            Rating: 5.0, // Default rating as individual package rating is not directly exposed
            ImageUrl: item.MainImageUrl?.Value,
            MapLink: item.MeetingPoint != null ? $"https://www.google.com/maps/search/?api=1&query={item.MeetingPoint.Latitude},{item.MeetingPoint.Longitude}" : null
        );
    }

    public static ChatbotCard FromLocation(Location item)
    {
        return new ChatbotCard(
            Id: item.Id.ToString(),
            ItemType: "place",
            Title: item.Name,
            Subtitle: $"{item.Category} — {(item.Description != null && item.Description.Length > 80 ? item.Description.Substring(0, 80) + "..." : item.Description)}",
            PriceLabel: null,
            Rating: (double)item.Rating,
            ImageUrl: item.MainImageUrl?.Value,
            MapLink: item.Coordinates != null ? $"https://www.google.com/maps/search/?api=1&query={item.Coordinates.Latitude},{item.Coordinates.Longitude}" : null
        );
    }
}
