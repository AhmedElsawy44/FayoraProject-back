namespace Fayora.Contracts.AuthModule.ConfirmChangePhone;

public record ConfirmChangePhoneRequest
(
    string NewPhoneNumber,
    string Code
);
