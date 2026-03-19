using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WarranTeaApi.Services;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"]
            ?? throw new InvalidOperationException("Missing config: AzureBlob:ConnectionString");
        var containerName = configuration["AzureBlob:ContainerName"]
            ?? throw new InvalidOperationException("Missing config: AzureBlob:ContainerName");
        _containerClient = new BlobContainerClient(connectionString, containerName);
    }

    public virtual async Task EnsureContainerExistsAsync()
    {
        await _containerClient.CreateIfNotExistsAsync();
    }

    public virtual async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        var blobName = $"{Guid.NewGuid()}/{fileName}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

        return blobClient.Uri.ToString();
    }

    public virtual async Task<Stream> DownloadAsync(string blobUrl)
    {
        var blobClient = _containerClient.GetBlobClient(GetBlobNameFromUrl(blobUrl));
        var response = await blobClient.DownloadStreamingAsync();
        return response.Value.Content;
    }

    public virtual async Task DeleteAsync(string blobUrl)
    {
        var blobClient = _containerClient.GetBlobClient(GetBlobNameFromUrl(blobUrl));
        await blobClient.DeleteIfExistsAsync();
    }

    private string GetBlobNameFromUrl(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        // Path is "/{container}/{blobName}" — skip the container segment
        var path = uri.AbsolutePath;
        var containerPrefix = $"/{_containerClient.Name}/";
        return path.StartsWith(containerPrefix)
            ? path[containerPrefix.Length..]
            : path.TrimStart('/');
    }
}