using Fayora.Domain.Common.Results;
using Fayora.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Fayora.Application.Common.Interfaces.Services.AuthModule;


public interface IUploadStrategy
{
    UploadContext Context { get; }
    Task<Result<List<string>>> UploadFilesAsync(List<IFormFile> files, CancellationToken cancellationToken);
}
public enum UploadContext { Profile, HousingUnit, Package, Verification }
