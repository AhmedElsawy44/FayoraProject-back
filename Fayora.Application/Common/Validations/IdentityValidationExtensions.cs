using Fayora.Application.Common.Interfaces;
using Fayora.Application.Common.Interfaces.Validations;
using FluentValidation;

namespace Fayora.Application.Common.Validations;

public static class IdentityValidationExtensions
{
    public static IRuleBuilderOptions<T, T> MustHaveExactlyOneIdentifier<T>(this IRuleBuilder<T, T> ruleBuilder)
        where T : ICheckBannedRequest
    {
        return ruleBuilder
            .Must(command =>
            {
                bool hasEmail = !string.IsNullOrWhiteSpace(command.Email);
                bool hasPhone = !string.IsNullOrWhiteSpace(command.PhoneNumber);
                return hasEmail ^ hasPhone;
            })
            .WithMessage("You must provide either Email or Phone Number, not both.")
            .WithName("Identifier");
    }
}