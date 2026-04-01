namespace Fayora.Contracts.AuthModule.ChangePassword;

public record ChangePasswordRequest(string? CurrentPassword, string NewPassword);