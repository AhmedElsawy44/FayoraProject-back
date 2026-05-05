namespace Fayora.Contracts.AdminModule.VerifyContent;

public record VerifyContentRequest(
    ItemType ItemType,
    bool IsApproved,
    string AdminNotes
);

public enum ItemType
{
    TourGuide,
    TourCompany,
    GuidePackage,
    Accommodation,
}
