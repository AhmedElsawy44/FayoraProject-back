namespace Fayora.Contracts.Auth.Requests;

public record VerifyResetPhonePasswordRequest(
    string PhoneNumber,
    string Code);
