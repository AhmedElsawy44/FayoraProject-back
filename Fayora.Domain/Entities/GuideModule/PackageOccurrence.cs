using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.GuideModule
{
    public class PackageOccurrence : BaseEntity<Guid>
    {
        public Guid PackageId { get; private set; }
        public DateTime Date { get; private set; }
        public int AvailableSeats { get; private set; }
        public OccurrenceStatus Status { get; private set; }

        public PackageOccurrence(Guid packageId, DateTime date, int availableSeats)
        {
            Id = Guid.NewGuid();
            PackageId = packageId;
            Date = date;
            AvailableSeats = availableSeats;
            Status = OccurrenceStatus.Available;
        }

        private PackageOccurrence() { }

        public Result<Success> ReserveSeats(int count)
        {
            if (Status == OccurrenceStatus.Cancelled)
                return Error.Failure("This occurrence is cancelled.");

            if (count > AvailableSeats)
                return Error.Validation("Not enough available seats.");

            AvailableSeats -= count;

            if (AvailableSeats == 0)
                Status = OccurrenceStatus.FullyBooked;

            return Result.Success;
        }

        public void ReleaseSeats(int count)
        {
            AvailableSeats += count;
            if (Status == OccurrenceStatus.FullyBooked)
                Status = OccurrenceStatus.Available;
        }

        public void Cancel()
        {
            Status = OccurrenceStatus.Cancelled;
        }
    }

}
