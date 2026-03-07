using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Services
{

    public record FacebookUserInfo(
        string Id,
        string? Email,
        string? Name,
        string? PictureUrl
    );

    public interface IFacebookAuthService
    {
        Task<FacebookUserInfo?> GetUserInfoAsync(string accessToken, CancellationToken ct = default);
    }
}
