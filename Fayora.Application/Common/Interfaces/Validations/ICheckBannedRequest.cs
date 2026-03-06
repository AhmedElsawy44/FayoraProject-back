namespace Fayora.Application.Common.Interfaces.Validations;

public interface ICheckBannedRequest
{
    string Identity { get; }
    string? Email { get; }
    string? PhoneNumber { get; }
    string DeviceId { get; }
    bool IsEmail { get; }
}