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
using Fayora.Contracts.Auth.AppleLogin;
using Fayora.Contracts.Auth.FacebookLogin;
using Fayora.Contracts.Auth.GoogleLogin;
using Fayora.Contracts.Auth.Login;
using Fayora.Contracts.Auth.RefreshToken;
using Fayora.Contracts.Auth.Register;
using Fayora.Contracts.Auth.RestoreAccount;
using Fayora.Contracts.Auth.Verify;

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
