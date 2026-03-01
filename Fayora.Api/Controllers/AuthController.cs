using Fayora.Application.Common.Authentication;
using Fayora.Application.Features.Auth.Commands.Register;
using Fayora.Contracts.Auth;
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
        var command = new RegisterCommand(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.Password, request.SimCountryIsoCode, request.TimeZone, request.DeviceInfo.DeviceId, request.DeviceInfo.FcmToken, request.DeviceInfo.DeviceLanguage);

        var authResult = await sender.Send(command);

        return authResult.Match(
            value => Ok(MapToAuthResponse(value)),
            errors => Problem(errors));
    }

    private static AuthResponse MapToAuthResponse(AuthResult authResult)
    {
        UserDto userDto = new(authResult.Id,
            authResult.FirstName,
            authResult.LastName,
            authResult.Email,
            authResult.PhoneNumber
        );
        return new AuthResponse(
            userDto,
            authResult.AccessToken,
            authResult.RefreshToken);
    }
}
