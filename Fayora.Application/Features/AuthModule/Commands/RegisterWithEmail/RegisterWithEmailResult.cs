namespace Fayora.Application.Features.AuthModule.Commands.RegisterWithEmail;

public record RegisterWithEmailResult(
    Guid UserId,
    string Email);
