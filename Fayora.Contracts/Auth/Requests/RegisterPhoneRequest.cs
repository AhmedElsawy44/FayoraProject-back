namespace Fayora.Contracts.Auth.Requests;

public record RegisterPhoneRequest(
    string PhoneNumber,
    string Password,
    CommunicationChannel DeliveryMethod
);

public enum CommunicationChannel
{
    Sms,
    WhatsApp
}
