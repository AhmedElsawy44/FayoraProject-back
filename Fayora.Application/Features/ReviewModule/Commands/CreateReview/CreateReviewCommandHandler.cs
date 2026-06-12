using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.ReviewModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Entities.ReviewModule;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.ReviewModule;
using Fayora.Domain.Errors;

namespace Fayora.Application.Features.ReviewModule.Commands.CreateReview;

public class CreateReviewCommandHandler(
    IReviewRepository reviewRepository,
    IBookingRepository bookingRepository,
    IClientContextProvider clientContextProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateReviewCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var userId = clientContextProvider.GetContext().UserId;

        var booking = await bookingRepository.GetBookingByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
        {
            return ReviewErrors.BookingNotFound;
        }

        if (booking.UserId != userId)
        {
            return ReviewErrors.BookingNotOwnedByUser;
        }

        if (booking.BookingStatus != BookingStatus.Completed)
        {
            return ReviewErrors.BookingNotCompleted;
        }

        var alreadyReviewed = await reviewRepository.HasReviewForBookingAsync(request.BookingId, cancellationToken);
        if (alreadyReviewed)
        {
            return ReviewErrors.AlreadyReviewed;
        }

        var targetType = booking.ServiceType switch
        {
            ServiceType.TourGuide => ReviewTargetType.TourGuide,
            ServiceType.GuidePackage => ReviewTargetType.GuidePackage,
            ServiceType.Accommodation => ReviewTargetType.HousingUnit,
            _ => throw new ArgumentOutOfRangeException(nameof(booking.ServiceType), booking.ServiceType, "Invalid booking service type.")
        };

        var reviewResult = Review.Create(
            booking.Id,
            userId,
            booking.ServiceId,
            targetType,
            request.Rating,
            request.Comment);

        if (reviewResult.IsError)
        {
            return reviewResult.Errors;
        }

        reviewRepository.AddReview(reviewResult.Value);
        await unitOfWork.CommitChangesAsync(cancellationToken);

        return reviewResult.Value.Id;
    }
}
