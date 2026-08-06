namespace NexHire.Infrastructure.Storage;

public interface IObjectStorageService
{
    Task<string> GenerateUploadUrlAsync(string key, string contentType, CancellationToken ct);
    Task<Stream> GetObjectStreamAsync(string key, CancellationToken ct);
}
