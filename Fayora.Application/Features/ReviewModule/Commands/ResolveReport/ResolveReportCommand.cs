using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.ReviewModule.Commands.ResolveReport;

public record ResolveReportCommand(Guid ReportId) : ICommand<Result<Success>>;
