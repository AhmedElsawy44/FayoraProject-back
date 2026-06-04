using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Services.RecommendationModule;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AdminModule.Commands.RefreshRecommendations;

public record RefreshRecommendationsCommand : ICommand<Result<PythonRefreshResult>>;
