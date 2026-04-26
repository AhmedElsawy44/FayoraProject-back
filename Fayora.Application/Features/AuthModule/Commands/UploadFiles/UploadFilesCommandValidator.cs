using FluentValidation;

namespace Fayora.Application.Features.AuthModule.Commands.UploadFiles;

public class UploadFilesCommandValidator : AbstractValidator<UploadFilesCommand>
{
    public UploadFilesCommandValidator()
    {
        RuleFor(x => x.Context)
            .IsInEnum().WithMessage("Invalid upload context.");

        RuleFor(x => x.Files)
            .Must(files => files.Count > 0).WithMessage("You must upload at least one file.")
            .Must(files => files.Count <= 10).WithMessage("You cannot upload more than 10 files at once.");

        RuleForEach(x => x.Files)
            .NotNull().WithMessage("File cannot be null.")
            .Must(file => file.Length > 0).WithMessage("File cannot be empty.");
    }
}
