namespace Fayora.Application.Common.Interfaces.Services.SharedModule;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
}