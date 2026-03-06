using Fayora.Application.Features.Auth.Commands.Register;
using Fayora.Application.Features.Auth.Commands.ResetPassword;
using Fayora.Application.Features.Auth.Commands.SendCode;
using Fayora.Application.Features.Auth.Commands.VerifyRegisterCode;
using Fayora.Application.Features.Auth.Commands.VerifyResetPasswordCode;
using Fayora.Application.Features.Auth.Queries.Login;
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
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Email, request.PhoneNumber, request.Password, request.DeviceId);

        var authResult = await sender.Send(command);

        return authResult.Match(Ok, Problem);
    }

    [HttpPost("verify-register-otp")]
    public async Task<IActionResult> VerifyRegisterationOtp(VerifyRegisterOtpRequest request)
    {
        var command = new VerifyRegisterCodeCommand(request.UserId, request.Email, request.PhoneNumber, request.Code, request.DeviceId, request.FcmToken, request.SimCountryIsoCode, request.TimeZone, request.DeviceLanguage);

        var verifyResult = await sender.Send(command);

        return verifyResult.Match(Ok, Problem);
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp(ResendOtpRequest request)
    {
        Enum.TryParse(request.OtpPurpose, out OtpPurpose purpose);
        var command = new SendCodeCommand(request.Email, request.PhoneNumber, request.DeviceId, purpose);

        var result = await sender.Send(command);

        return result.Match(
        _ => Ok(),
        errors => Problem(errors));
    }

    [HttpPost("verify-reset-password-otp")]
    public async Task<IActionResult> VerifyResetPasswordOtp(VerifyResetPasswordOtpRequest request)
    {
        var command = new VerifyResetPasswordCodeCommand(request.Email, request.PhoneNumber, request.DeviceId, request.Code);

        var verifyResult = await sender.Send(command);

        return verifyResult.Match(Ok, Problem);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request.Email, request.PhoneNumber, request.DeviceId, request.ResetPasswordToken, request.NewPassword);

        var result = await sender.Send(command);

        return result.Match(_ => Ok(), errors => Problem(errors));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = new LoginQuery(request.Email, request.PhoneNumber, request.Password, request.DeviceId, request.FcmToken, request.DeviceLanguage);

        var result = await sender.Send(command);

        return result.Match(Ok, Problem);
    }
}
