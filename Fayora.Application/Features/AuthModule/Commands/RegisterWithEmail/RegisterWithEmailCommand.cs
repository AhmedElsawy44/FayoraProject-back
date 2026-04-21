using Fayora.Application.Abstractions.Messaging;
using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
namespace Fayora.Application.Features.AuthModule.Commands.RegisterWithEmail;

public record RegisterWithEmailCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string DeviceId
    ) : ICommand<Result<RegisterWithEmailResult>>, ICheckBannedRequest;
