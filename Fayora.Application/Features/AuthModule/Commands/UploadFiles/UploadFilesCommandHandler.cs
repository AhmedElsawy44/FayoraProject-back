using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.UploadFiles;

public class UploadFilesCommandValidator : AbstractValidator<UploadFilesCommand>
{
    public UploadFilesCommandValidator()
    {
    }
}
