using AutoMapper;
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
using Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithEmail;
using Fayora.Application.Features.AuthModule.Commands.RestoreAccountWithPhone;
using Fayora.Application.Features.AuthModule.Commands.VerifyEmail;
using Fayora.Application.Features.AuthModule.Commands.VerifyPhone;
using Fayora.Application.Features.AuthModule.Queries.GetUser;
using Fayora.Contracts.AuthModule.AppleLogin;
using Fayora.Contracts.AuthModule.ConfirmChangeEmail;
using Fayora.Contracts.AuthModule.ConfirmChangePhone;
using Fayora.Contracts.AuthModule.FacebookLogin;
using Fayora.Contracts.AuthModule.GetUser;
using Fayora.Contracts.AuthModule.GoogleLogin;
using Fayora.Contracts.AuthModule.Login;
using Fayora.Contracts.AuthModule.RefreshToken;
using Fayora.Contracts.AuthModule.Register;
using Fayora.Contracts.AuthModule.RestoreAccount;
using Fayora.Contracts.AuthModule.Verify;

namespace Fayora.Application.Common.Mapper;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<RegisterWithEmailResult, EmailRegisterResponse>();
        CreateMap<RegisterWithPhoneResult, PhoneRegisterResponse>();
        CreateMap<LoginWithEmailResult, EmailLoginResponse>();
        CreateMap<LoginWithPhoneResult, PhoneLoginResponse>();
        CreateMap<VerifyEmailResult, EmailVerifyResponse>();
        CreateMap<VerifyPhoneResult, PhoneVerifyResponse>();
        CreateMap<LoginWithFacebookResult, FacebookLoginResponse>();
        CreateMap<LoginWithGoogleResult, GoogleLoginResponse>();
        CreateMap<LoginWithAppleResult, AppleLoginResponse>();
        CreateMap<RestoreAccountWithEmailResult, RestoreAccountWithEmailResponse>();
        CreateMap<RestoreAccountWithPhoneResult, RestoreAccountWithPhoneResponse>();
        CreateMap<RefreshTokenResult, RefreshTokenResponse>();
        CreateMap<ConfirmChangeEmailResult, ConfirmChangeEmailResponse>();
        CreateMap<ConfirmChangePhoneResult, ConfirmChangePhoneResponse>();

        CreateMap<GetUserResult, GetUserResponse>()
            .ForCtorParam("Gender", opt => opt.MapFrom(src =>
                src.Gender.HasValue ? src.Gender.Value.ToString() : null))

            .ForCtorParam("PreferredLanguage", opt => opt.MapFrom(src =>
                src.PreferredLanguage.HasValue ? src.PreferredLanguage.Value.ToString() : null))

            .ForCtorParam("LanguageProficiencies", opt => opt.MapFrom(src =>
                src.LanguageProficiencies != null
                    ? src.LanguageProficiencies.Select(lp =>
                        new LanguageProficiencyDto(
                            lp.Language.ToString(),
                            lp.ProficiencyLevel
                        )).ToList()
                    : new List<LanguageProficiencyDto>()
            ));
    }
}
