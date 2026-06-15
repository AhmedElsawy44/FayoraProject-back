using System;
using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule;

public class OptionalActivity
{
    public Guid Id { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal AdditionalPrice { get; private set; }
    public FileUrl ImageUrl { get; private set; } = null!;

    private OptionalActivity() { }

    private OptionalActivity(Guid id, string description, decimal additionalPrice, FileUrl imageUrl)
    {
        Id = id;
        Description = description;
        AdditionalPrice = additionalPrice;
        ImageUrl = imageUrl;
    }

    public static Result<OptionalActivity> Create(string description, decimal additionalPrice, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Error.Validation("OptionalActivity.InvalidDescription", "Description is required.");

        if (additionalPrice < 0)
            return Error.Validation("OptionalActivity.InvalidPrice", "Additional price cannot be negative.");

        var imageUrlResult = FileUrl.Create(imageUrl);
        if (imageUrlResult.IsError)
            return imageUrlResult.Errors;

        return new OptionalActivity(Guid.NewGuid(), description, additionalPrice, imageUrlResult.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not OptionalActivity other) return false;
        return Id == other.Id 
            && Description == other.Description 
            && AdditionalPrice == other.AdditionalPrice 
            && ImageUrl.Value == other.ImageUrl.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Description, AdditionalPrice, ImageUrl.Value);
    }
}
