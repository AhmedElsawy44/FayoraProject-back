using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fayora.Application.Common.Interfaces.Persistences.AccommodationModule;
using Fayora.Application.Common.Interfaces.Persistences.BookingModule;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Persistences.NotificationModule;
using Fayora.Application.Common.Interfaces.Services.SharedModule;
using Fayora.Domain.Common.Events.BookingModule;
using Fayora.Domain.Entities.Booking;
using Fayora.Domain.Entities.NotificationModule;
using Fayora.Domain.Enums.BookingModule;
using Fayora.Domain.Enums.SharedModule;
using MediatR;
using static Fayora.Application.Common.Interfaces.Persistences.AccommodationModule.IHousingUnitRepository;
using static Fayora.Application.Common.Interfaces.Persistences.GuideModule.IPackageRepository;
using static Fayora.Application.Common.Interfaces.Persistences.IdentityModule.IUserRepository;

namespace Fayora.Application.Features.NotificationModule.Events;

public class BookingEventsHandler(
    IBookingRepository bookingRepository,
    IUserRepository userRepository,
    IHousingUnitRepository housingUnitRepository,
    IPackageRepository packageRepository,
    INotificationRepository notificationRepository,
    IFirebaseNotificationService firebaseNotificationService,
    IUnitOfWork unitOfWork)
    : INotificationHandler<BookingCreatedEvent>,
      INotificationHandler<BookingPaidEvent>,
      INotificationHandler<BookingDepositPaidEvent>,
      INotificationHandler<BookingCanceledEvent>,
      INotificationHandler<BookingCompletedEvent>
{
    private const string TypeBookingDetails = "booking_details";

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetBookingByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null) return;

        var serviceName = await GetServiceNameAsync(booking, cancellationToken);
        var tourist = await userRepository.GetUserByIdAsync(booking.UserId, new UserQueryOptions(IsReadOnly: true), cancellationToken);
        var touristName = tourist != null ? $"{tourist.FirstName} {tourist.LastName}" : "سائح";

        // 1. Notify Tourist
        await SendNotificationAsync(
            booking.UserId,
            "حجز جديد معلق الدفع",
            $"يرجى إكمال الدفع لتأكيد حجزك لـ {serviceName}.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        // 2. Notify Provider
        await SendNotificationAsync(
            booking.ServiceProviderId,
            "طلب حجز جديد معلق",
            $"لديك طلب حجز جديد معلق الدفع لـ {serviceName} من {touristName}.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }

    public async Task Handle(BookingPaidEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetBookingByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null) return;

        var serviceName = await GetServiceNameAsync(booking, cancellationToken);
        var tourist = await userRepository.GetUserByIdAsync(booking.UserId, new UserQueryOptions(IsReadOnly: true), cancellationToken);
        var touristName = tourist != null ? $"{tourist.FirstName} {tourist.LastName}" : "سائح";

        // 1. Notify Tourist
        await SendNotificationAsync(
            booking.UserId,
            "تأكيد الحجز",
            $"تم تأكيد حجزك لـ {serviceName} بنجاح! يمكنك الآن استخدام كود الـ QR.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        // 2. Notify Provider
        await SendNotificationAsync(
            booking.ServiceProviderId,
            "تأكيد الدفع وحجز جديد",
            $"تم دفع حجز {serviceName} بواسطة {touristName} وتأكيده بنجاح.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }

    public async Task Handle(BookingDepositPaidEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetBookingByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null) return;

        var serviceName = await GetServiceNameAsync(booking, cancellationToken);
        var tourist = await userRepository.GetUserByIdAsync(booking.UserId, new UserQueryOptions(IsReadOnly: true), cancellationToken);
        var touristName = tourist != null ? $"{tourist.FirstName} {tourist.LastName}" : "سائح";

        // 1. Notify Tourist
        await SendNotificationAsync(
            booking.UserId,
            "تأكيد عربون الحجز",
            $"تم دفع عربون حجزك لـ {serviceName} بنجاح! يرجى دفع المبلغ المتبقي عند الوصول.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        // 2. Notify Provider
        await SendNotificationAsync(
            booking.ServiceProviderId,
            "حجز جديد (عربون مدفوع)",
            $"تم تأكيد حجز {serviceName} ودفع العربون بواسطة {touristName}.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }

    public async Task Handle(BookingCanceledEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetBookingByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null) return;

        var serviceName = await GetServiceNameAsync(booking, cancellationToken);
        var tourist = await userRepository.GetUserByIdAsync(booking.UserId, new UserQueryOptions(IsReadOnly: true), cancellationToken);
        var touristName = tourist != null ? $"{tourist.FirstName} {tourist.LastName}" : "سائح";

        // 1. Notify Tourist
        await SendNotificationAsync(
            booking.UserId,
            "إلغاء الحجز",
            $"تم إلغاء حجزك لـ {serviceName} بنجاح.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        // 2. Notify Provider
        await SendNotificationAsync(
            booking.ServiceProviderId,
            "إلغاء حجز",
            $"قام العميل {touristName} بإلغاء حجزه لـ {serviceName}.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }

    public async Task Handle(BookingCompletedEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetBookingByIdAsync(notification.BookingId, cancellationToken);
        if (booking == null) return;

        var serviceName = await GetServiceNameAsync(booking, cancellationToken);
        var tourist = await userRepository.GetUserByIdAsync(booking.UserId, new UserQueryOptions(IsReadOnly: true), cancellationToken);
        var touristName = tourist != null ? $"{tourist.FirstName} {tourist.LastName}" : "سائح";

        // 1. Notify Tourist
        await SendNotificationAsync(
            booking.UserId,
            "حجز مكتمل",
            $"تم إكمال حجزك لـ {serviceName} بنجاح. شكراً لاستخدامك Fayora!",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        // 2. Notify Provider
        await SendNotificationAsync(
            booking.ServiceProviderId,
            "حجز مكتمل",
            $"تم تأكيد استلام المبلغ بالكامل وإكمال الحجز لـ {serviceName} مع العميل {touristName}.",
            TypeBookingDetails,
            booking.Id.ToString(),
            cancellationToken);

        await unitOfWork.CommitChangesAsync(cancellationToken);
    }

    private async Task<string> GetServiceNameAsync(Booking booking, CancellationToken ct)
    {
        try
        {
            if (booking.ServiceType == ServiceType.GuidePackage)
            {
                var package = await packageRepository.GetPackageByIdAsync(booking.ServiceId, new PackageQueryOptions(ReadOnly: true), ct);
                return package?.Title ?? "باقة رحلات";
            }
            if (booking.ServiceType == ServiceType.Accommodation)
            {
                var unit = await housingUnitRepository.GetUnitByIdAsync(booking.ServiceId, new UnitQueryOptions(IsReadOnly: true), ct);
                return unit?.Title ?? "وحدة سكنية";
            }
            if (booking.ServiceType == ServiceType.TourGuide)
            {
                return "رحلة إرشادية";
            }
        }
        catch
        {
            // Fail safe default
        }
        return "خدمة سياحية";
    }

    private async Task SendNotificationAsync(
        Guid userId,
        string title,
        string body,
        string type,
        string entityId,
        CancellationToken ct)
    {
        // 1. Save In-App Notification
        var inAppNotification = InAppNotification.Create(userId, title, body, type, entityId);
        await notificationRepository.AddInAppNotificationAsync(inAppNotification, ct);

        // 2. Fetch Device Tokens and Send Push Notification
        var tokens = await notificationRepository.GetTokensByUserIdAsync(userId, ct);
        if (tokens != null && tokens.Any())
        {
            var dataPayload = new Dictionary<string, string>
            {
                { "type", type },
                { "entityId", entityId }
            };

            await firebaseNotificationService.SendBroadcastAsync(
                title,
                body,
                imageUrl: null,
                tokens,
                ct,
                dataPayload);
        }
    }
}
