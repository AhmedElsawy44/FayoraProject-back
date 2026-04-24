using Microsoft.AspNetCore.Http;

namespace Fayora.Contracts.AuthModule.UploadFile;

public record UploadFileRequest
(
    List<IFormFile> Files,
    string Context
);
