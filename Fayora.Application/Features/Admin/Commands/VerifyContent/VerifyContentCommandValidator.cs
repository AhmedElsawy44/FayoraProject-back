using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Admin.Commands.ApproveVerification
{
    public class VerifyContentCommandValidator : AbstractValidator<VerifyContentCommand>
    {
        public VerifyContentCommandValidator()
        {
        }
    }
}
