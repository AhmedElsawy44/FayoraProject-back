namespace Fayora.Application.Features.Auth.Commands.RegisterWithEmail;

public record RegisterWithEmailResult(
    Guid UserId,
    string Email);
