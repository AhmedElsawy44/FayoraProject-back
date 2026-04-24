using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.GuideModule
{
    // tourist request guide for his tour
    public class GuideRequest : AuditableEntity<Guid>
    {
        public Guid TripPlanId { get; private set; } // the trip plan that the tourist want to request a guide for [FK]
        public Guid UserId { get; private set; } // the tourist who made the request [FK]
        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string CurrencyCode { get; private set; } = null!;
        public decimal BudgetAmount { get; private set; }
        public int NumberOfPeople { get; private set; }
        public GeoPoint MeetingPoint { get; private set; } = null!;
        public GuideRequestStatus Status { get; private set; } = GuideRequestStatus.Open;


        private readonly List<GuideOffer> _guideOffers = [];
        public IReadOnlyCollection<GuideOffer> GuideOffers => _guideOffers.AsReadOnly();


        private GuideRequest(
            Guid tripPlanId,
            Guid userId,
            string title,
            string description,
            DateTime startDate,
            DateTime endDate,
            decimal budgetAmount,
            int numberOfPeople,
            GeoPoint meetingPoint,
            string currencyCode = "EGP"
            )
        {
            TripPlanId = tripPlanId;
            UserId = userId;
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            BudgetAmount = budgetAmount;
            NumberOfPeople = numberOfPeople;
            MeetingPoint = meetingPoint;
            CurrencyCode = currencyCode;
            Status = GuideRequestStatus.Open;
        }


        private GuideRequest() { }


        public static Result<GuideRequest> Create(
            Guid tripPlanId, Guid userId, string title,
            string description, DateTime startDate, DateTime endDate,
            decimal budgetAmount, int numberOfPeople,
            GeoPoint meetingPoint, string currencyCode = "EGP")
        {
            if (string.IsNullOrWhiteSpace(title))
                return Error.Validation("GuideRequest.EmptyTitle", "Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(description))
                return Error.Validation("GuideRequest.EmptyDescription", "Description cannot be empty.");

            if (startDate >= endDate)
                return Error.Validation("GuideRequest.InvalidDates", "Start date must be before end date.");

            if (startDate < DateTime.UtcNow)
                return Error.Validation("GuideRequest.PastDate", "Start date cannot be in the past.");

            if (budgetAmount <= 0)
                return Error.Validation("GuideRequest.InvalidBudget", "Budget must be greater than zero.");

            if (numberOfPeople <= 0)
                return Error.Validation("GuideRequest.InvalidPeople", "Number of people must be greater than zero.");

            return new GuideRequest(tripPlanId, userId, title, description,
                startDate, endDate, budgetAmount, numberOfPeople, meetingPoint, currencyCode);
        }


        public Result<Success> Cancel()
        {
            if (Status != GuideRequestStatus.Open)
                return Error.Conflict("GuideRequest.NotOpen", "Only open requests can be cancelled.");
            Status = GuideRequestStatus.Cancelled;
            Updated();
            return Result.Success;
        }

        public Result<Success> Expire()
        {
            if (Status != GuideRequestStatus.Open)
                return Error.Conflict("GuideRequest.NotOpen", "Only open requests can expire.");
            Status = GuideRequestStatus.Expired;
            Updated();
            return Result.Success;
        }

        public Result<Success> Accept(Guid acceptedOfferId)
        {
            if (Status != GuideRequestStatus.Open)
                return Error.Conflict("GuideRequest.NotOpen", "Only open requests can be accepted.");

            foreach (var offer in _guideOffers.Where(o => o.Id != acceptedOfferId))
                offer.Reject();

            Status = GuideRequestStatus.Accepted;
            Updated();
            return Result.Success;
        }


        public Result<Success> UpdateBudget(decimal newBudget)
        {
            if (Status != GuideRequestStatus.Open)
                return Error.Conflict("GuideRequest.Closed", "Cannot update budget for a closed request.");
            if (newBudget <= 0)
                return Error.Validation("GuideRequest.InvalidBudget", "Budget must be greater than zero.");
            BudgetAmount = newBudget;
            Updated();
            return Result.Success;
        }

        public Result<Success> UpdateDetails(string title, string description, int numberOfPeople)
        {
            if (Status != GuideRequestStatus.Open)
                return Error.Conflict("GuideRequest.Closed", "Cannot update a closed request.");
            if (string.IsNullOrWhiteSpace(title))
                return Error.Validation("GuideRequest.EmptyTitle", "Title cannot be empty.");
            Title = title;
            Description = description;
            NumberOfPeople = numberOfPeople;
            Updated();
            return Result.Success;
        }

        public Result<Success> UpdateDates(DateTime startDate, DateTime endDate)
        {
            if (Status != GuideRequestStatus.Open)
                return Error.Conflict("GuideRequest.Closed", "Cannot update a closed request.");
            if (startDate >= endDate)
                return Error.Validation("GuideRequest.InvalidDates", "Start date must be before end date.");
            if (startDate < DateTime.UtcNow)
                return Error.Validation("GuideRequest.PastDate", "Start date cannot be in the past.");
            StartDate = startDate;
            EndDate = endDate;
            Updated();
            return Result.Success;
        }

        public Result<Success> UpdateMeetingPoint(GeoPoint newMeetingPoint)
        {
            if (Status != GuideRequestStatus.Open)
                return Error.Conflict("GuideRequest.Closed", "Cannot update a closed request.");
            MeetingPoint = newMeetingPoint;
            Updated();
            return Result.Success;
        }


    }
}
