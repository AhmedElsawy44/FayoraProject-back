using AutoMapper;
using Fayora.Application.Features.Auth.Commands.LoginWithApple;
using Fayora.Application.Features.Auth.Commands.LoginWithEmail;
using Fayora.Application.Features.Auth.Commands.LoginWithFacebook;
using Fayora.Application.Features.Auth.Commands.LoginWithGoogle;
using Fayora.Application.Features.Auth.Commands.LoginWithPhone;
using Fayora.Application.Features.Auth.Commands.RegisterWithEmail;
using Fayora.Application.Features.Auth.Commands.RegisterWithPhone;
using Fayora.Application.Features.Auth.Commands.VerifyEmail;
using Fayora.Application.Features.Auth.Commands.VerifyPhone;
using Fayora.Contracts.Auth.Responses;

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
    }
}
