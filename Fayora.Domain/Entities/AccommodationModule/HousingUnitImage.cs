using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.AccommodationModule;

public class HousingUnitImage
{
    public Guid Id { get; private set; }
    public Guid HousingUnitId { get; private set; }
    public string ImageUrl { get; private set; }

    public HousingUnitImage(Guid housingUnitId, string imageUrl)
    {
        Id = Guid.NewGuid();
        HousingUnitId = housingUnitId;
        ImageUrl = imageUrl;
    }

    private HousingUnitImage() { }
}