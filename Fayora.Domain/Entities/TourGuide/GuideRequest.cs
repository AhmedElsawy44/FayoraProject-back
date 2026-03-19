using Fayora.Domain.Common.Entity;
using Fayora.Domain.Enums.TourGuideModule;
using Fayora.Domain.ValueObjects;

namespace Fayora.Domain.Entities.TourGuide
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
        public GeoPoint MeetingPoint { get; private set; } = new GeoPoint(0, 0);
        public GuideRequestStatus Status { get; private set; } = GuideRequestStatus.Open;


        private readonly List<GuideOffer> _guideOffers = [];
        public IReadOnlyCollection<GuideOffer> GuideOffers => _guideOffers.AsReadOnly();


        private GuideRequest(Guid tripPalnId,
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
            TripPlanId = tripPalnId;
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


        public static GuideRequest CreateGuideRequest(
           Guid tripPlanId,
           Guid userId,
           string title,
           string description,
           DateTime startDate,
           DateTime endDate,
           decimal budgetAmount,
           int numberOfPeople,
           GeoPoint meetingPoint,
           string currencyCode = "EGP")
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidOperationException("Description cannot be empty.");

            if (startDate >= endDate)
                throw new InvalidOperationException("Start date must be before end date.");

            if (startDate < DateTime.UtcNow)
                throw new InvalidOperationException("Start date cannot be in the past.");

            if (budgetAmount <= 0)
                throw new InvalidOperationException("Budget must be greater than zero.");

            if (numberOfPeople <= 0)
                throw new InvalidOperationException("Number of people must be greater than zero.");

            return new GuideRequest(tripPlanId, userId, title, description, startDate, endDate, budgetAmount, numberOfPeople, meetingPoint, currencyCode);
        }



        public void Cancel()
        {
            if (Status != GuideRequestStatus.Open)
                throw new InvalidOperationException("Only open requests can be cancelled.");
            Status = GuideRequestStatus.Cancelled;
            Updated();
        }

        public void Expire()
        {
            if (Status != GuideRequestStatus.Open)
                throw new InvalidOperationException("Only open requests can expire.");
            Status = GuideRequestStatus.Expired;
            Updated();
        }

        public void Accept(Guid acceptedOfferId)
        {
            if (Status != GuideRequestStatus.Open)
                throw new InvalidOperationException("Only open requests can be accepted.");

            // reject all other offers automatically
            foreach (var offer in _guideOffers.Where(o => o.Id != acceptedOfferId))
                offer.Reject();

            Status = GuideRequestStatus.Accepted;
            Updated();
        }


        public void UpdateBudget(decimal newBudget)
        {
            if (Status != GuideRequestStatus.Open)
                throw new InvalidOperationException("Cannot update budget for a closed request.");
            if (newBudget <= 0)
                throw new InvalidOperationException("Budget must be greater than zero.");
            BudgetAmount = newBudget;
            Updated();
        }


        public void UpdateMeetingPoint(GeoPoint newMeetingPoint)
        {
            if (Status != GuideRequestStatus.Open)
                throw new InvalidOperationException("Cannot update a closed request.");
            MeetingPoint = newMeetingPoint;
            Updated();
        }


        public void UpdateDates(DateTime startDate, DateTime endDate)
        {
            if (Status != GuideRequestStatus.Open)
                throw new InvalidOperationException("Cannot update a closed request.");
            if (startDate >= endDate)
                throw new InvalidOperationException("Start date must be before end date.");
            if (startDate < DateTime.UtcNow)
                throw new InvalidOperationException("Start date cannot be in the past.");
            StartDate = startDate;
            EndDate = endDate;
            Updated();
        }


        public void UpdateDetails(string title, string description, int numberOfPeople)
        {
            if (Status != GuideRequestStatus.Open)
                throw new InvalidOperationException("Cannot update a closed request.");
            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException("Title cannot be empty.");
            Title = title;
            Description = description;
            NumberOfPeople = numberOfPeople;
            Updated();
        }


    }
}
