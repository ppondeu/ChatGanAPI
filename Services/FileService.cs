using ChatApi.Interfaces;

namespace ChatApi.Services;

public class FileService : IFileService
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".jfif"];
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public async Task<string> SaveFileAsync(IFormFile file, string uploadDirectory)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file uploaded.");
        }

        // Ensure the upload directory exists
        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }

        var fileName = $"{Guid.NewGuid()}-{file.FileName}";
        var filePath = Path.Combine(uploadDirectory, fileName);

        // Save the file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName; // Return the file path (could be used to store in the database or send back to the user)
    }

    public async Task<string> ValidateAndSaveFileAsync(IFormFile file, string uploadDirectory)
    {
        // Validate the file size
        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException("File size exceeds the allowed limit of 10MB.");
        }

        // Validate the file extension
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!_allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, .png, .gif files are allowed.");
        }

        // If validation passes, save the file
        return await SaveFileAsync(file, uploadDirectory);
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        return Task.FromResult(File.Exists(filePath));
    }
}
