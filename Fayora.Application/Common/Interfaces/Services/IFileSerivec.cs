using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Application.Common.Interfaces.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
}
