using System;

namespace ChatApi.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string uploadDirectory);
    Task<string> ValidateAndSaveFileAsync(IFormFile file, string uploadDirectory);
    Task<bool> FileExistsAsync(string filePath);
}

