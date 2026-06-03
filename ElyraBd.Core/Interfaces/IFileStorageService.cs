namespace ElyraBd.Core.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveProductImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    void DeleteFile(string relativePath);
}
