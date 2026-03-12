using AutoMapper;
using Fayora.Application.Features.AuthModule.Commands.LoginWithApple;
using Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;
using Fayora.Application.Features.AuthModule.Commands.LoginWithFacebook;
using Fayora.Application.Features.AuthModule.Commands.LoginWithGoogle;
using Fayora.Application.Features.AuthModule.Commands.LoginWithPhone;
using Fayora.Application.Features.AuthModule.Commands.RefreshToken;
using Fayora.Application.Features.AuthModule.Commands.RegisterWithEmail;
using Fayora.Application.Features.AuthModule.Commands.RegisterWithPhone;
using Fayora.Application.Features.AuthModule.Commands.ResetPasswordEmail;
using Fayora.Application.Features.AuthModule.Commands.ResetPasswordPhone;
using Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail;
using Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone;
using Fayora.Application.Features.AuthModule.Commands.SendEmailCode;
using Fayora.Application.Features.AuthModule.Commands.SendPhoneCode;
using Fayora.Application.Features.AuthModule.Commands.VerifyEmail;
using Fayora.Application.Features.AuthModule.Commands.VerifyPhone;
using Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordEmailCode;
using Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordPhoneCode;
using Fayora.Contracts.AuthModule.AppleLogin;
using Fayora.Contracts.AuthModule.FacebookLogin;
using Fayora.Contracts.AuthModule.GoogleLogin;
using Fayora.Contracts.AuthModule.Login;
using Fayora.Contracts.AuthModule.RefreshToken;
using Fayora.Contracts.AuthModule.Register;
using Fayora.Contracts.AuthModule.ResetPassword;
using Fayora.Contracts.AuthModule.RestoreAccount;
using Fayora.Contracts.AuthModule.SendCode;
using Fayora.Contracts.AuthModule.Verify;
using Fayora.Domain.Enums.IdentityModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fayora.Api.Controllers;

[Route("api/[controller]")]
public class AuthController(ISender sender, IMapper mapper) : ApiController
{
    #region 1. Registration & Verification

    [HttpPost("register/email")]
    public async Task<IActionResult> RegisterWithEmail(
        [FromBody] EmailRegisterRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new RegisterWithEmailCommand(request.Email, request.Password, deviceId);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<EmailRegisterResponse>(value)),
            Problem
        );
    }

    [HttpPost("register/phone")]
    public async Task<IActionResult> RegisterWithPhone(
        [FromBody] PhoneRegisterRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<CodeDeliveryMethod>(request.DeliveryMethod, true, out var deliveryMethod))
            return BadRequest("Invalid Delivery Method");

        var command = new RegisterWithPhoneCommand(request.PhoneNumber, request.Password, deliveryMethod, deviceId);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<PhoneRegisterResponse>(value)),
            Problem
        );
    }

    [HttpPost("register/verify/email")]
    public async Task<IActionResult> VerifyEmailRegistration(
        [FromBody] EmailVerifyRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new VerifyEmailCommand(
            request.Email, request.Code, deviceId,
            request.FcmToken, request.SimCountryIsoCode, request.TimeZone, request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<EmailVerifyResponse>(value)),
            Problem
        );
    }

    [HttpPost("register/verify/phone")]
    public async Task<IActionResult> VerifyPhoneRegistration(
        [FromBody] PhoneVerifyRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new VerifyPhoneCommand(
            request.PhoneNumber, request.Code, deviceId,
            request.FcmToken, request.SimCountryIsoCode, request.TimeZone, request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<PhoneVerifyResponse>(value)),
            Problem
        );
    }

    #endregion

    #region 2. Login

    [HttpPost("login/email")]
    public async Task<IActionResult> LoginWithEmail(
        [FromBody] EmailLoginRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new LoginWithEmailCommand(
            request.Email, request.Password, deviceId, request.FcmToken, request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<EmailLoginResponse>(value)),
            Problem
        );
    }

    [HttpPost("login/phone")]
    public async Task<IActionResult> LoginWithPhone(
        [FromBody] PhoneLoginRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new LoginWithPhoneCommand(
            request.PhoneNumber, request.Password, deviceId, request.FcmToken, request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<PhoneLoginResponse>(value)),
            Problem
        );
    }

    [HttpPost("login/facebook")]
    public async Task<IActionResult> FacebookLogin(
        [FromBody] FacebookLoginRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new LoginWithFacebookCommand(
            request.AccessToken,
            deviceId,
            request.FcmToken,
            request.SimCountryIsoCode,
            request.TimeZone,
            request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<FacebookLoginResponse>(value)),
            Problem);
    }

    [HttpPost("login/google")]
    public async Task<IActionResult> GoogleLogin(
        [FromBody] GoogleLoginRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new LoginWithGoogleCommand(
            request.AccessToken,
            deviceId,
            request.FcmToken,
            request.SimCountryIsoCode,
            request.TimeZone,
            request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<GoogleLoginResponse>(value)),
            Problem
        );
    }

    [HttpPost("login/apple")]
    public async Task<IActionResult> AppleLogin(
        [FromBody] AppleLoginRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new LoginWithAppleCommand(
            request.IdToken,
            deviceId,
            request.FcmToken,
            request.SimCountryIsoCode,
            request.TimeZone,
            request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<AppleLoginResponse>(value)),
            Problem
        );
    }

    #endregion

    #region 3. OTP Management

    [HttpPost("otp/send/email")]
    public async Task<IActionResult> SendEmailOtp(
        [FromBody] SendEmailCodeRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<CodePurpose>(request.Purpose.ToString(), true, out var purpose))
            return BadRequest("Invalid Code Purpose");

        var command = new SendEmailCodeCommand(request.Email, deviceId, purpose);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("otp/send/phone")]
    public async Task<IActionResult> SendPhoneOtp(
        [FromBody] SendPhoneCodeRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<CodePurpose>(request.Purpose.ToString(), true, out var purpose))
            return BadRequest("Invalid Code Purpose");

        if (!Enum.TryParse<CodeDeliveryMethod>(request.DeliveryMethod.ToString(), true, out var deliveryMethod))
            return BadRequest("Invalid Delivery Method");

        var command = new SendPhoneCodeCommand(request.PhoneNumber, deviceId, purpose, deliveryMethod);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    #endregion

    #region 4. Password Reset Journey

    [HttpPost("password/reset/verify/email")]
    public async Task<IActionResult> VerifyEmailPasswordReset(
        [FromBody] VerifyEmailResetPasswordRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new VerifyResetPasswordEmailCodeCommand(request.Email, deviceId, request.Code);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            token => Ok(new VerifyResetPasswordResponse(token)),
            Problem
        );
    }

    [HttpPost("password/reset/verify/phone")]
    public async Task<IActionResult> VerifyPhonePasswordReset(
        [FromBody] VerifyResetPhonePasswordRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new VerifyResetPasswordPhoneCodeCommand(request.PhoneNumber, deviceId, request.Code);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            token => Ok(new VerifyResetPasswordResponse(token)),
            Problem
        );
    }

    [HttpPost("password/reset/email")]
    public async Task<IActionResult> ResetPasswordWithEmail(
        [FromBody] ResetEmailPasswordRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordEmailCommand(request.Email, request.ResetToken, request.NewPassword, deviceId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("password/reset/phone")]
    public async Task<IActionResult> ResetPasswordWithPhone(
        [FromBody] ResetPhonePasswordRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordPhoneCommand(request.PhoneNumber, request.ResetToken, request.NewPassword, deviceId);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    #endregion

    #region 5. Rsstore Account


    [HttpPost("restore-account/email")]
    public async Task<IActionResult> RestoreAccountWithEmail(
    [FromBody] RestoreAccountWithEmailRequest request,
    [FromHeader(Name = "X-Device-Id")] string deviceId,
    CancellationToken cancellationToken)
    {
        var command = new RestoreAccountWithEmailCommand(
            request.Email,
            request.Code,
            deviceId,
            request.FcmToken,
            request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<RestoreAccountWithEmailResponse>(value)),
            Problem);
    }


    [HttpPost("restore-account/phone")]
    public async Task<IActionResult> RestoreAccountWithPhone(
    [FromBody] RestoreAccountWithPhoneRequest request,
    [FromHeader(Name = "X-Device-Id")] string deviceId,
    CancellationToken cancellationToken)
    {
        var command = new RestoreAccountWithPhoneCommand(
            request.PhoneNumber,
            request.Code,
            deviceId,
            request.FcmToken,
            request.DeviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<RestoreAccountWithPhoneResponse>(value)),
            Problem);
    }

    #endregion

    #region 6. Refresh Token

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
    [FromBody] RefreshTokenRequest request,
    [FromHeader(Name = "X-Device-Id")] string deviceId,
    CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(
            request.RefreshToken,
            deviceId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<RefreshTokenResponse>(value)),
            Problem);
    }

    #endregion


}