using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;

namespace Fayora.Application.Features.AuthModule.Commands.ChangePhone;

public record ChangePhoneCommand(string PhoneNumber, string Password, string DeviceId) : ICheckBannedRequest, IRequest<Result<ChangePhoneResult>>;
