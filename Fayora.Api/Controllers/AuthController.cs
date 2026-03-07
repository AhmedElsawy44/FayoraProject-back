using Fayora.Application.Features.Auth.Commands.Login;
using Fayora.Application.Features.Auth.Commands.Register;
using Fayora.Application.Features.Auth.Commands.ResetPassword;
using Fayora.Application.Features.Auth.Commands.SendCode;
using Fayora.Application.Features.Auth.Commands.VerifyRegisterCode;
using Fayora.Application.Features.Auth.Commands.VerifyResetPasswordCode;
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
        if(!Enum.TryParse(request.DeliveryMethod.ToString(), out Domain.Enums.CodeDeliveryMethod deliveryMethod))
        {
            return BadRequest("Invalid Delivery Method");
        }

        var command = new RegisterCommand(request.Email, request.PhoneNumber, request.Password, request.DeviceId, deliveryMethod);
        var authResult = await sender.Send(command);
        return authResult.Match(Ok, Problem);
    }

    [HttpPost("register/verify")]
    public async Task<IActionResult> VerifyRegistration(VerifyRegisterOtpRequest request)
    {
        var command = new VerifyRegisterCodeCommand(request.UserId, request.Email, request.PhoneNumber, request.Code, request.DeviceId, request.FcmToken, request.SimCountryIsoCode, request.TimeZone, request.DeviceLanguage);
        var verifyResult = await sender.Send(command);
        return verifyResult.Match(Ok, Problem);
    }

    [HttpPost("otp/send")]
    public async Task<IActionResult> SendOtp(SendCodeRequest request)
    {
        if (!Enum.TryParse(request.OtpPurpose, out CodePurpose purpose) || !Enum.TryParse(request.DeliveryMethod.ToString(), out Domain.Enums.CodeDeliveryMethod deliveryMethod))
        {
            return BadRequest("Invalid OTP Purpose or Delivery Method");
        }

        var command = new SendCodeCommand(request.Email, request.PhoneNumber, request.DeviceId, purpose, deliveryMethod);
        var result = await sender.Send(command);

        return result.Match(_ => Ok(), errors => Problem(errors));
    }

    [HttpPost("password/reset/verify")]
    public async Task<IActionResult> VerifyPasswordReset(VerifyResetPasswordCodeRequest request)
    {
        var command = new VerifyResetPasswordCodeCommand(request.Email, request.PhoneNumber, request.DeviceId, request.Code);
        var verifyResult = await sender.Send(command);
        return verifyResult.Match(Ok, Problem);
    }

    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var command = new ResetPasswordCommand(request.Email, request.PhoneNumber, request.DeviceId, request.ResetPasswordToken, request.NewPassword);
        var result = await sender.Send(command);
        return result.Match(_ => Ok(), errors => Problem(errors));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.PhoneNumber, request.Password, request.DeviceId, request.FcmToken, request.DeviceLanguage);
        var result = await sender.Send(command);
        return result.Match(Ok, Problem);
    }
}