using Fayora.Application.Common.Abstractions.Messaging;
using Fayora.Application.Common.Factories;
using Fayora.Domain.Common.Results;

namespace Fayora.Application.Features.AuthModule.Commands.UploadFiles;

public class UploadFilesCommandHandler(UploadStrategyFactory strategyFactory)
    : ICommandHandler<UploadFilesCommand, Result<List<string>>>
{
    public async Task<Result<List<string>>> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
    {
        if (request.Files == null || !request.Files.Any())
        {
            return Error.Validation("Files.Empty", "No files were provided for upload.");
        }

        var strategy = strategyFactory.GetStrategy(request.Context);

        var result = await strategy.UploadFilesAsync(request.Files, cancellationToken);

        return result;
    }
}
