using Fayora.Application.Common.Interfaces.Validations;
using Fayora.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Features.Auth.Commands.LoginWithGoogle;

public record LoginWithGoogleCommand(string IdToken, string DeviceId, string FcmToken, string? SimCountryIsoCode,
string TimeZone,, string DeviceLanguage) : IRequest<Result<LoginWithGoogleResult>>, ICheckBannedRequest;
