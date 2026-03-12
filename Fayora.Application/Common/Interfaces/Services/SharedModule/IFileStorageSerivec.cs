namespace Fayora.Application.Common.Interfaces.Services.SharedServices;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName);
}