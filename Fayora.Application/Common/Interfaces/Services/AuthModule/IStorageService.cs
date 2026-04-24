using Microsoft.AspNetCore.Http;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;

public interface IStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken);
}
