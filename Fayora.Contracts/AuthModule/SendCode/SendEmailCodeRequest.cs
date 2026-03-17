namespace Fayora.Contracts.AuthModule.SendCode;

public record SendEmailCodeRequest(
    string Email,
     string Purpose
);