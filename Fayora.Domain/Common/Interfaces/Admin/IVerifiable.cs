using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;

namespace Fayora.Domain.Common.Interfaces.Admin;

public interface IVerifiable
{
    ItemStatus Status { get; }
    string? AdminNotes { get; }
    Result<Success> Approve();
    Result<Success> Reject(string adminNotes);
}
