using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Persistences.GuideModule;
using Fayora.Application.Common.Interfaces.Persistences.IdentityModule;
using Fayora.Application.Common.Interfaces.Services.AuthModule;
using Fayora.Domain.Entities.GuideModule;
using MediatR;

namespace Fayora.Application.Features.TourGuideModule.Commands.CreateGuideWeeklySchedule;

public class CreateGuideWeeklyScheduleCommandHandler(
    IClientContextProvider clientContextProvider,
    IGuideWeeklyScheduleRepository guideWeeklyScheduleRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateGuideWeeklyScheduleCommand, Unit>
{
    public async Task<Unit> Handle(CreateGuideWeeklyScheduleCommand request, CancellationToken cancellationToken)
    {
        var guideId = clientContextProvider.GetContext().UserId;

        var guideWeeklySchedule = new GuideWeeklySchedule(guideId, request.DayOfWeek, request.StartTime, request.EndTime);

        guideWeeklyScheduleRepository.AddSchedule(guideWeeklySchedule);

        await unitOfWork.CommitChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
