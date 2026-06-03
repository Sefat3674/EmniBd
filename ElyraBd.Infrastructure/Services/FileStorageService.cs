using ElyraBd.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ElyraBd.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadRoot;
    private readonly ILogger<FileStorageService> _logger;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    public FileStorageService(IWebHostEnvironment environment, ILogger<FileStorageService> logger)
    {
        _uploadRoot = Path.Combine(environment.WebRootPath, "uploads", "products");
        _logger = logger;
        Directory.CreateDirectory(_uploadRoot);
    }

    public async Task<string> SaveProductImageAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException("Invalid image file type.");

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var physicalPath = Path.Combine(_uploadRoot, safeName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await fileStream.CopyToAsync(stream, cancellationToken);

        return $"/uploads/products/{safeName}";
    }

    public void DeleteFile(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;

        var fileName = Path.GetFileName(relativePath);
        var physicalPath = Path.Combine(_uploadRoot, fileName);
        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
            _logger.LogInformation("Deleted file {Path}", physicalPath);
        }
    }
}
