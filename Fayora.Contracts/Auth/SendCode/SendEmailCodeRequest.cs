namespace Fayora.Contracts.Auth.SendCode;

public record SendEmailCodeRequest(
    string Email,
     string Purpose
);