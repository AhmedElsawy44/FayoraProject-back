namespace Fayora.Contracts.Auth.Requests;

public record SendEmailCodeRequest(
    string Email,
     string Purpose
);