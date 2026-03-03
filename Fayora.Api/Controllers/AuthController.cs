using Fayora.Application.Features.Auth.Commands.Register;
using Fayora.Application.Features.Auth.Commands.ResendRegisterOtp;
using Fayora.Application.Features.Auth.Commands.VerifyRegisterOtp;
using Fayora.Application.Features.Auth.Common;
using Fayora.Contracts.Auth;
using Fayora.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ISender sender) : ApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var command = new RegisterCommand(request.Email, request.PhoneNumber, request.Password, request.SimCountryIsoCode, request.TimeZone, request.DeviceInfo.DeviceId, request.DeviceInfo.DeviceLanguage);

        var authResult = await sender.Send(command);

        return authResult.Match(
            value => Ok(MapToAuthResponse(value)),
            errors => Problem(errors));
    }

    [HttpPost("verify-register-otp")]
    public async Task<IActionResult> VerifyRegisterationOtp(VerifyRegisterOtpRequestDto request)
    {
        var command = new VerifyRegisterOtpCommand(request.UserId, request.Email, request.PhoneNumber, request.SimCountryIsoCode, request.Otp, request.DeviceInfoDto.DeviceId, request.DeviceInfoDto.FcmToken);

        var verifyResult = await sender.Send(command);

        return verifyResult.Match(Ok, Problem);
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(ResendOtpRequestDto request)
    {
        Enum.TryParse(request.OtpPurpose, out OtpPurpose purpose);
        var command = new ResendOtpCommand(request.UserId, request.Email, request.PhoneNumber, request.SimCountryIsoCode, purpose);

        var result = await sender.Send(command);

        return result.Match(
        _ => Ok(),
        errors => Problem(errors));
    }

    private static RegisterResponseDto MapToAuthResponse(RegisterResult authResult)
    {
        IdentifierType identifierType = !String.IsNullOrWhiteSpace(authResult.Email)
            ? IdentifierType.Email
            : IdentifierType.Phone;
        string identifier = authResult.Email ?? authResult.PhoneNumber!;

        return new RegisterResponseDto(authResult.Id, identifier, identifierType.ToString());
    }
}
