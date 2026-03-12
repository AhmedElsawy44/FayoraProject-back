using AutoMapper;
using Fayora.Application.Features.Auth.Commands.LoginWithApple;
using Fayora.Application.Features.Auth.Commands.LoginWithEmail;
using Fayora.Application.Features.Auth.Commands.LoginWithFacebook;
using Fayora.Application.Features.Auth.Commands.LoginWithGoogle;
using Fayora.Application.Features.Auth.Commands.LoginWithPhone;
using Fayora.Application.Features.Auth.Commands.RefreshToken;
using Fayora.Application.Features.Auth.Commands.RegisterWithEmail;
using Fayora.Application.Features.Auth.Commands.RegisterWithPhone;
using Fayora.Application.Features.Auth.Commands.RestoreAccountWithEmail;
using Fayora.Application.Features.Auth.Commands.RestoreAccountWithPhone;
using Fayora.Application.Features.Auth.Commands.VerifyEmail;
using Fayora.Application.Features.Auth.Commands.VerifyPhone;
using Fayora.Contracts.AuthModule.AppleLogin;
using Fayora.Contracts.AuthModule.FacebookLogin;
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
    }
}
