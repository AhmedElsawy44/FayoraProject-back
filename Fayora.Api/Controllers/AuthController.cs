using AutoMapper;
using Fayora.Application.Features.Auth.Commands.FacebookLogin;
using Fayora.Application.Features.Auth.Commands.LoginWithEmail;
using Fayora.Application.Features.Auth.Commands.LoginWithPhone;
using Fayora.Application.Features.Auth.Commands.RegisterWithEmail;
using Fayora.Application.Features.Auth.Commands.RegisterWithPhone;
using Fayora.Application.Features.Auth.Commands.ResetPasswordEmail;
using Fayora.Application.Features.Auth.Commands.ResetPasswordPhone;
using Fayora.Application.Features.Auth.Commands.SendEmailCode;
using Fayora.Application.Features.Auth.Commands.SendPhoneCode;
using Fayora.Application.Features.Auth.Commands.VerifyEmail;
using Fayora.Application.Features.Auth.Commands.VerifyPhone;
using Fayora.Application.Features.Auth.Commands.VerifyResetPasswordEmailCode;
using Fayora.Application.Features.Auth.Commands.VerifyResetPasswordPhoneCode;
using Fayora.Contracts.Auth.Requests;
using Fayora.Contracts.Auth.Responses;
using Fayora.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ISender sender, IMapper mapper) : ApiController
{
    #region 1. Registration & Verification

    [HttpPost("register/email")]
    public async Task<IActionResult> RegisterWithEmail(
        [FromBody] RegisterEmailRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new RegisterWithEmailCommand(request.Email, request.Password, deviceId);
        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<RegisterEmailResponse>(value)),
            Problem
        );
    }

    [HttpPost("register/phone")]
    public async Task<IActionResult> RegisterWithPhone(
        [FromBody] RegisterPhoneRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        if (!Enum.TryParse<CodeDeliveryMethod>(request.DeliveryMethod, true, out var deliveryMethod))
            return BadRequest("Invalid Delivery Method");

        var command = new RegisterWithPhoneCommand(request.PhoneNumber, request.Password, deliveryMethod, deviceId);
        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<RegisterPhoneResponse>(value)),
            Problem
        );
    }

    [HttpPost("register/verify/email")]
    public async Task<IActionResult> VerifyEmailRegistration(
        [FromBody] VerifyEmailRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new VerifyEmailCommand(
            request.UserId, request.Email, request.Code, deviceId,
            request.FcmToken, request.SimCountryIsoCode, request.TimeZone, request.DeviceLanguage);

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<VerifyEmailResponse>(value)),
            Problem
        );
    }

    [HttpPost("register/verify/phone")]
    public async Task<IActionResult> VerifyPhoneRegistration(
        [FromBody] VerifyPhoneRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new VerifyPhoneCommand(
            request.UserId, request.PhoneNumber, request.Code, deviceId,
            request.FcmToken, request.SimCountryIsoCode, request.TimeZone, request.DeviceLanguage);

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<VerifyPhoneResponse>(value)),
            Problem
        );
    }

    #endregion

    #region 2. Login

    [HttpPost("login/email")]
    public async Task<IActionResult> LoginWithEmail(
        [FromBody] LoginEmailRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new LoginWithEmailCommand(
            request.Email, request.Password, deviceId, request.FcmToken, request.DeviceLanguage);

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<LoginEmailResponse>(value)),
            Problem
        );
    }

    [HttpPost("login/phone")]
    public async Task<IActionResult> LoginWithPhone(
        [FromBody] LoginPhoneRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new LoginWithPhoneCommand(
            request.PhoneNumber, request.Password, deviceId, request.FcmToken, request.DeviceLanguage);

        var result = await sender.Send(command);

        return result.Match(
            value => Ok(mapper.Map<LoginPhoneResponse>(value)),
            Problem
        );
    }

    #endregion

    #region 3. LoginWithFacebook

    [HttpPost("facebook-login")]
    public async Task<IActionResult> FacebookLogin(FacebookLoginRequest request)
    {
        var command = new LoginWithFacebookCommand(
            request.AccessToken,
            request.DeviceId,
            request.FcmToken,
            request.DeviceLanguage);

        var result = await sender.Send(command);

        return result.Match(Ok, Problem);
    }

    #endregion

    #region 4. OTP Management

    [HttpPost("otp/send/email")]
    public async Task<IActionResult> SendEmailOtp(
        [FromBody] SendEmailCodeRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        if (!Enum.TryParse<CodePurpose>(request.Purpose.ToString(), true, out var purpose))
            return BadRequest("Invalid Code Purpose");

        var command = new SendEmailCodeCommand(request.Email, deviceId, purpose);
        var result = await sender.Send(command);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("otp/send/phone")]
    public async Task<IActionResult> SendPhoneOtp(
        [FromBody] SendPhoneCodeRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        if (!Enum.TryParse<CodePurpose>(request.Purpose.ToString(), true, out var purpose))
            return BadRequest("Invalid Code Purpose");

        if (!Enum.TryParse<CodeDeliveryMethod>(request.DeliveryMethod.ToString(), true, out var deliveryMethod))
            return BadRequest("Invalid Delivery Method");

        var command = new SendPhoneCodeCommand(request.PhoneNumber, deviceId, purpose, deliveryMethod);
        var result = await sender.Send(command);

        return result.Match(_ => Ok(), Problem);
    }

    #endregion

    #region 5. Password Reset Journey

    [HttpPost("password/reset/verify/email")]
    public async Task<IActionResult> VerifyEmailPasswordReset(
        [FromBody] VerifyResetPasswordEmailRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new VerifyResetPasswordEmailCodeCommand(request.Email, deviceId, request.Code);
        var result = await sender.Send(command);

        return result.Match(
            token => Ok(new VerifyResetPasswordResponse(token)),
            Problem
        );
    }

    [HttpPost("password/reset/verify/phone")]
    public async Task<IActionResult> VerifyPhonePasswordReset(
        [FromBody] VerifyResetPasswordPhoneRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new VerifyResetPasswordPhoneCodeCommand(request.PhoneNumber, deviceId, request.Code);
        var result = await sender.Send(command);

        return result.Match(
            token => Ok(new VerifyResetPasswordResponse(token)),
            Problem
        );
    }

    [HttpPost("password/reset/email")]
    public async Task<IActionResult> ResetPasswordWithEmail(
        [FromBody] ResetPasswordEmailRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new ResetPasswordEmailCommand(request.Email, request.ResetToken, request.NewPassword, deviceId);
        var result = await sender.Send(command);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("password/reset/phone")]
    public async Task<IActionResult> ResetPasswordWithPhone(
        [FromBody] ResetPasswordPhoneRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId)
    {
        var command = new ResetPasswordPhoneCommand(request.PhoneNumber, request.ResetToken, request.NewPassword, deviceId);
        var result = await sender.Send(command);

        return result.Match(_ => Ok(), Problem);
    }

    #endregion

}