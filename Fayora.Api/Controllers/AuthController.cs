using AutoMapper;
using MediatR;
using Fayora.Application.Features.AuthModule.Commands.ChangeEmail;
using Fayora.Application.Features.AuthModule.Commands.ChangePassword;
using Fayora.Application.Features.AuthModule.Commands.ChangePhone;
using Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;
using Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;
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
using Fayora.Application.Features.AuthModule.Commands.UpdateAccount;
using Fayora.Application.Features.AuthModule.Commands.VerifyDeleteEmailAccount;
using Fayora.Application.Features.AuthModule.Commands.VerifyDeletePhoneAccountCommand;
using Fayora.Application.Features.AuthModule.Commands.VerifyEmail;
using Fayora.Application.Features.AuthModule.Commands.VerifyPhone;
using Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordEmailCode;
using Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordPhoneCode;
using Fayora.Application.Features.AuthModule.Queries.GetUser;
using Fayora.Contracts.AuthModule.AppleLogin;
using Fayora.Contracts.AuthModule.ChangeEmail;
using Fayora.Contracts.AuthModule.ChangePassword;
using Fayora.Contracts.AuthModule.ChangePhone;
using Fayora.Contracts.AuthModule.ConfirmChangeEmail;
using Fayora.Contracts.AuthModule.ConfirmChangePhone;
using Fayora.Contracts.AuthModule.FacebookLogin;
using Fayora.Contracts.AuthModule.GetUser;
using Fayora.Contracts.AuthModule.GoogleLogin;
using Fayora.Contracts.AuthModule.Login;
using Fayora.Contracts.AuthModule.RefreshToken;
using Fayora.Contracts.AuthModule.Register;
using Fayora.Contracts.AuthModule.ResetPassword;
using Fayora.Contracts.AuthModule.RestoreAccount;
using Fayora.Contracts.AuthModule.SendCode;
using Fayora.Contracts.AuthModule.UpdateAccount;
using Fayora.Contracts.AuthModule.Verify;
using Fayora.Contracts.AuthModule.VerifyDeleteEmailAccount;
using Fayora.Contracts.AuthModule.VerifyDeletePhoneAccount;
using Fayora.Domain.Enums.IdentityModule;
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
        var command = new RegisterWithEmailCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            deviceId);

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

        var command = new RegisterWithPhoneCommand(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Password,
            deliveryMethod, deviceId);

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
            request.FirstName,
            request.LastName,
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
        if (!Enum.TryParse<CodePurpose>(request.Purpose, true, out var purpose))
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
        if (!Enum.TryParse<CodePurpose>(request.Purpose, true, out var purpose))
            return BadRequest("Invalid Code Purpose");

        if (!Enum.TryParse<CodeDeliveryMethod>(request.DeliveryMethod, true, out var deliveryMethod))
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
        var command =
            new ResetPasswordPhoneCommand(request.PhoneNumber, request.ResetToken, request.NewPassword, deviceId);
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

    #region 7. Account Management

    [HttpGet("account/profile")]
    public async Task<IActionResult> GetAccountProfile(CancellationToken cancellationToken)
    {
        var query = new GetUserCommand();

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<GetUserResponse>(value)),
            Problem
        );
    }

    [HttpPost("account/delete/verify/email")]
    public async Task<IActionResult> VerifyDeleteEmailAccount(
        [FromBody] VerifyDeleteEmailAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new VerifyDeleteEmailAccountCommand(request.Code);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("account/delete/verify/phone")]
    public async Task<IActionResult> VerifyDeletePhoneAccount(
        [FromBody] VerifyDeletePhoneAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new VerifyDeletePhoneAccountCommand(request.Code);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPut("account/profile")]
    public async Task<IActionResult> UpdateAccountProfile(
    [FromBody] UpdateAccountRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateAccountCommand(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            request.Gender,
            request.NationalityCode,
            request.ProfileImageUrl,
            request.Description,
            request.PreferredLanguage,
            request.UserLanguages,
            request.TimeZone
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPost("account/email/change")]
    public async Task<IActionResult> ChangeEmail(
        [FromBody] ChangeEmailRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new ChangeEmailCommand(
            request.Email,
            request.Password,
            deviceId
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<ChangeEmailResponse>(value)),
            Problem
        );
    }

    [HttpPost("account/phone/change")]
    public async Task<IActionResult> ChangePhone(
        [FromBody] ChangePhoneRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse(request.CodeDeliveryMethod, true, out CodeDeliveryMethod codeDeliveryMethod))
            return BadRequest("Invalid CodeDeliveryMethod value.");

        var command = new ChangePhoneCommand(
            request.Phone,
            request.Password,
            deviceId,
            codeDeliveryMethod
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<ChangePhoneResponse>(value)),
            Problem
        );
    }

    [HttpPost("account/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(request.CurrentPassword, request.NewPassword);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpPost("account/confirm-change-email")]
    public async Task<IActionResult> ConfirmChangeEmail(
        [FromBody] ConfirmChangeEmailRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmChangeEmailCommand(deviceId, request.NewEmail, request.Code);

        var result = await sender.Send(command, cancellationToken);
        return result.Match(
            value => Ok(mapper.Map<ConfirmChangeEmailResponse>(value)),
            Problem
        );
    }

    [HttpPost("account/confirm-change-phone")]
    public async Task<IActionResult> ConfirmChangePhone(
        [FromBody] ConfirmChangePhoneRequest request,
        [FromHeader(Name = "X-Device-Id")] string deviceId,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmChangePhoneCommand(deviceId, request.NewPhoneNumber, request.Code);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<ConfirmChangePhoneResponse>(value)),
            Problem
        );
    }

    #endregion

}