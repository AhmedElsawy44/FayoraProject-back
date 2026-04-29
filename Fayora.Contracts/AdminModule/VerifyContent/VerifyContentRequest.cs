namespace Fayora.Contracts.AdminModule.ApproveVerification;

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
