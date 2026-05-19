using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Recommendation.Functions.Storage;

public sealed class BlobStorageClient
{
    private readonly IConfiguration _configuration;

    public BlobStorageClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SaveResponseAsync(
        Guid requestId,
        string response,
        CancellationToken cancellationToken = default)
    {
        var connectionString =
            _configuration["BlobStorage:ConnectionString"]
            ?? throw new InvalidOperationException(
                "BlobStorage:ConnectionString is not configured.");

        var containerName =
            _configuration["BlobStorage:ContainerName"]
            ?? throw new InvalidOperationException(
                "BlobStorage:ContainerName is not configured.");

        var blobServiceClient =
            new BlobServiceClient(connectionString);

        var containerClient =
            blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient =
            containerClient.GetBlobClient(
                $"{requestId}/response.json");

        using var stream = new MemoryStream(
            System.Text.Encoding.UTF8.GetBytes(response));

        await blobClient.UploadAsync(
            stream,
            overwrite: true,
            cancellationToken);
    }
}