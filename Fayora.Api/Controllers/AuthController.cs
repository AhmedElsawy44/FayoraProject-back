using AutoMapper;
using Fayora.Application.Features.AuthModule.Commands.ChangeEmail;
using Fayora.Application.Features.AuthModule.Commands.ChangePassword;
using Fayora.Application.Features.AuthModule.Commands.ChangePhone;
using Fayora.Application.Features.AuthModule.Commands.ConfirmChangeEmail;
using Fayora.Application.Features.AuthModule.Commands.ConfirmChangePhone;
using Fayora.Application.Features.AuthModule.Commands.LoginWithEmail;
using Fayora.Application.Features.AuthModule.Commands.LoginWithPhone;
using Fayora.Application.Features.AuthModule.Commands.LoginWithSocial;
using Fayora.Application.Features.AuthModule.Commands.RefreshToken;
using Fayora.Application.Features.AuthModule.Commands.RegisterWithEmail;
using Fayora.Application.Features.AuthModule.Commands.RegisterWithPhone;
using Fayora.Application.Features.AuthModule.Commands.ResetPassword;
using Fayora.Application.Features.AuthModule.Commands.RestoreAccount;
using Fayora.Application.Features.AuthModule.Commands.SendEmailCode;
using Fayora.Application.Features.AuthModule.Commands.SendPhoneCode;
using Fayora.Application.Features.AuthModule.Commands.UpdateAccount;
using Fayora.Application.Features.AuthModule.Commands.VerifyDeleteAccount;
using Fayora.Application.Features.AuthModule.Commands.VerifyEmail;
using Fayora.Application.Features.AuthModule.Commands.VerifyPhone;
using Fayora.Application.Features.AuthModule.Commands.VerifyResetPasswordCode;
using Fayora.Application.Features.AuthModule.Queries.GetUser;
using Fayora.Contracts.AuthModule.ChangeEmail;
using Fayora.Contracts.AuthModule.ChangePassword;
using Fayora.Contracts.AuthModule.ChangePhone;
using Fayora.Contracts.AuthModule.ConfirmChangeEmail;
using Fayora.Contracts.AuthModule.ConfirmChangePhone;
using Fayora.Contracts.AuthModule.GetUser;
using Fayora.Contracts.AuthModule.Login;
using Fayora.Contracts.AuthModule.RefreshToken;
using Fayora.Contracts.AuthModule.Register;
using Fayora.Contracts.AuthModule.ResetPassword;
using Fayora.Contracts.AuthModule.RestoreAccount;
using Fayora.Contracts.AuthModule.SendCode;
using Fayora.Contracts.AuthModule.SocialLogin;
using Fayora.Contracts.AuthModule.UpdateAccount;
using Fayora.Contracts.AuthModule.Verify;
using Fayora.Contracts.AuthModule.VerifyDeleteEmailAccount;
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
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
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
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, deliveryMethod) = EnumParser.TryParseEnum<CodeDeliveryMethod>(request.DeliveryMethod);

        if (!success)
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
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, deviceLanguage) = EnumParser.TryParseEnum<Language>(request.DeviceLanguage);

        if (!success)
            return BadRequest("Invalid Device Language");

        var command = new VerifyEmailCommand(
            request.Email, request.Code, deviceId,
            request.FcmToken, request.SimCountryIsoCode, request.TimeZone, deviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<EmailVerifyResponse>(value)),
            Problem
        );
    }

    [HttpPost("register/verify/phone")]
    public async Task<IActionResult> VerifyPhoneRegistration(
        [FromBody] PhoneVerifyRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, deviceLanguage) = EnumParser.TryParseEnum<Language>(request.DeviceLanguage);

        if (!success)
            return BadRequest("Invalid Device Language");

        var command = new VerifyPhoneCommand(
            request.PhoneNumber, request.Code, deviceId,
            request.FcmToken, request.SimCountryIsoCode, request.TimeZone, deviceLanguage);

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
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, deviceLanguage) = EnumParser.TryParseEnum<Language>(request.DeviceLanguage);

        if (!success)
            return BadRequest("Invalid Device Language");

        var command = new LoginWithEmailCommand(
            request.Email, request.Password, deviceId, request.FcmToken, deviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<EmailLoginResponse>(value)),
            Problem
        );
    }

    [HttpPost("login/phone")]
    public async Task<IActionResult> LoginWithPhone(
        [FromBody] PhoneLoginRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, deviceLanguage) = EnumParser.TryParseEnum<Language>(request.DeviceLanguage);

        if (!success)
            return BadRequest("Invalid Device Language");

        var command = new LoginWithPhoneCommand(
            request.PhoneNumber, request.Password, deviceId, request.FcmToken, deviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<PhoneLoginResponse>(value)),
            Problem
        );
    }


    [HttpPost("login/social")]
    public async Task<IActionResult> SocialLogin(
    [FromBody] SocialLoginRequest request,
    [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
    CancellationToken cancellationToken)
    {
        var (success, deviceLanguage) = EnumParser.TryParseEnum<Language>(request.DeviceLanguage);

        if (!success)
            return BadRequest("Invalid Device Language");

        var (providerSuccess, provider) = EnumParser.TryParseEnum<IdentityProvider>(request.SocialProvider);

        if (!providerSuccess)
            return BadRequest("Invalid Social Provider");

        var command = new LoginWithSocialCommand(
            request.Token,
            request.FirstName,
            request.LastName,
            deviceId,
            request.FcmToken,
            request.SimCountryIsoCode,
            request.TimeZone,
            deviceLanguage,
            provider);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<SocialLoginResponse>(value)),
            Problem
        );
    }

    #endregion

    #region 3. OTP Management

    [HttpPost("otp/send/email")]
    public async Task<IActionResult> SendEmailOtp(
        [FromBody] SendEmailCodeRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, purpose) = EnumParser.TryParseEnum<CodePurpose>(request.Purpose);

        if (!success)
            return BadRequest("Invalid Code Purpose");

        var command = new SendEmailCodeCommand(request.Email, deviceId, purpose);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    [HttpPost("otp/send/phone")]
    public async Task<IActionResult> SendPhoneOtp(
        [FromBody] SendPhoneCodeRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, purpose) = EnumParser.TryParseEnum<CodePurpose>(request.Purpose);

        if (!success)
            return BadRequest("Invalid Code Purpose");

        var (success2, deliveryMethod) = EnumParser.TryParseEnum<CodeDeliveryMethod>(request.DeliveryMethod);

        if (!success2)
            return BadRequest("Invalid Delivery Method");

        var command = new SendPhoneCodeCommand(request.PhoneNumber, deviceId, purpose, deliveryMethod);
        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    #endregion

    #region 4. Password Reset Journey

    [HttpPost("password/reset/verify/")]
    public async Task<IActionResult> VerifyEmailPasswordReset(
        [FromBody] VerifyResetPasswordRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, method) = EnumParser.TryParseEnum<CodeDeliveryMethod>(request.CodeDeliveryMethod);

        if (!success)
            return BadRequest("Invalid Code Delivery Method.");

        var command = new VerifyResetPasswordCodeCommand(request.Code, request.Identifier, method, deviceId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            token => Ok(new VerifyResetPasswordResponse(token)),
            Problem
        );
    }

    [HttpPost("password/reset/")]
    public async Task<IActionResult> ResetPasswordWithEmail(
        [FromBody] ResetEmailPasswordRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, codeDeliveryMethod) = EnumParser.TryParseEnum<CodeDeliveryMethod>(request.CodeDeliveryMethod);

        if (!success)
            return BadRequest("Invalid Code Delivery Method.");

        var command = new ResetPasswordCommand(request.Identifier, request.ResetToken, request.NewPassword, codeDeliveryMethod, deviceId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => Ok(), Problem);
    }

    #endregion

    #region 5. Restore Account

    [HttpPost("restore-account/")]
    public async Task<IActionResult> RestoreAccountWithEmail(
        [FromBody] RestoreAccountRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (languageSuccess, deviceLanguage) = EnumParser.TryParseEnum<Language>(request.DeviceLanguage);

        if (!languageSuccess)
            return BadRequest("Invalid Device Language");

        var (codeSuccess, codeDeliveryMethod) = EnumParser.TryParseEnum<CodeDeliveryMethod>(request.CodeDeliveryMethod);

        if (!codeSuccess)
            return BadRequest("Invalid Code Delivery Method.");

        var command = new RestoreAccountCommand(
            request.Identifier,
            request.Code,
            codeDeliveryMethod,
            deviceId,
            request.FcmToken,
            deviceLanguage);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<RestoreAccountResponse>(value)),
            Problem);
    }

    #endregion

    #region 6. Refresh Token

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
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
        var query = new GetUserQuery();

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            value => Ok(mapper.Map<GetUserResponse>(value)),
            Problem
        );
    }

    [HttpPost("account/delete/verify/")]
    public async Task<IActionResult> VerifyDeleteEmailAccount(
        [FromBody] VerifyDeleteEmailAccountRequest request,
        CancellationToken cancellationToken)
    {
        var (success, codeDeliveryMethod) = EnumParser.TryParseEnum<CodeDeliveryMethod>(request.CodeDeliveryMethod);

        if (!success)
            return BadRequest("Invalid Code Delivery Method.");

        var command = new VerifyDeleteAccountCommand(request.Code, codeDeliveryMethod);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("account/profile")]
    public async Task<IActionResult> UpdateAccountProfile(
    [FromBody] UpdateAccountRequest request,
    CancellationToken cancellationToken)
    {
        Gender? gender = null;
        if (!string.IsNullOrWhiteSpace(request.Gender))
        {
            var (success, parsedGender) = EnumParser.TryParseEnum<Gender>(request.Gender);
            if (!success)
                return BadRequest("Invalid Gender");
            gender = parsedGender;
        }

        Language? preferredLanguage = null;
        if (!string.IsNullOrWhiteSpace(request.PreferredLanguage))
        {
            var (success, parsedLanguage) = EnumParser.TryParseEnum<Language>(request.PreferredLanguage);
            if (!success)
                return BadRequest("Invalid Preferred Language");
            preferredLanguage = parsedLanguage;
        }

        var userLanguages = new List<UserLanguageProficiencyDto>();
        if (request.UserLanguages is not null && request.UserLanguages.Count != 0)
        {
            foreach (var dto in request.UserLanguages)
            {
                var (langSuccess, language) = EnumParser.TryParseEnum<Language>(dto.Language);
                if (!langSuccess)
                    return BadRequest("Invalid User Language");

                userLanguages.Add(new UserLanguageProficiencyDto(language, dto.ProficiencyLevel));
            }
        }

        var command = new UpdateAccountCommand(
            request.FirstName,
            request.LastName,
            request.BirthDate,
            gender,
            request.NationalityCode,
            request.ProfileImageUrl,
            request.Description,
            preferredLanguage,
            userLanguages,
            request.TimeZone
        );

        var result = await sender.Send(command, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("account/email/change")]
    public async Task<IActionResult> ChangeEmail(
        [FromBody] ChangeEmailRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
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
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
        CancellationToken cancellationToken)
    {
        var (success, codeDeliveryMethod) = EnumParser.TryParseEnum<CodeDeliveryMethod>(request.CodeDeliveryMethod);
        if (!success)
            return BadRequest("Invalid Code Delivery Method.");

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

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("account/confirm-change-email")]
    public async Task<IActionResult> ConfirmChangeEmail(
        [FromBody] ConfirmChangeEmailRequest request,
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
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
        [FromHeader(Name = AppHeaders.DeviceId)] string deviceId,
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