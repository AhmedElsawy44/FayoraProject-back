using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.BookingModule;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.BookingModule;

namespace Fayora.Application.Features.BookingModule.Commands.CreatePackageBooking;

public record CreatePackageBookingCommand(
    Guid PackageId,
    DateOnly BookingDate,
    int Adults,
    int Children,
    PaymentMethodType PaymentMethodType)
    : ICommand<Result<string>>;
