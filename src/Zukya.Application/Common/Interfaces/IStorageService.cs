namespace Zukya.Application.Common.Interfaces;

public interface IStorageService
{
    public Task Delete(string filePath, CancellationToken cancellationToken);
    public Task<string> Upload(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken
    );
    public string GetPublicUrl(string filePath);
}
