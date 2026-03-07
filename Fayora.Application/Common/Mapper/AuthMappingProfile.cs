using AutoMapper;
using Fayora.Application.Features.Auth.Commands.LoginWithEmail;
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
        CreateMap<RegisterWithEmailResult, RegisterEmailResponse>();
        CreateMap<RegisterWithPhoneResult, RegisterPhoneResponse>();
        CreateMap<LoginWithEmailResult, LoginEmailResponse>();
        CreateMap<LoginWithPhoneResult, LoginPhoneResponse>();
        CreateMap<VerifyEmailResult, VerifyEmailResponse>();
        CreateMap<VerifyPhoneResult, VerifyPhoneResponse>();
    }
}
