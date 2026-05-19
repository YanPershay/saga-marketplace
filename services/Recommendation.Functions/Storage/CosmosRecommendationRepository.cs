using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Recommendation.Functions.Models;

namespace Recommendation.Functions.Storage;

public sealed class CosmosRecommendationRepository
{
    private readonly IConfiguration _configuration;

    public CosmosRecommendationRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SaveAsync(
        RecommendationResult result,
        CancellationToken cancellationToken = default)
    {
        var connectionString =
            _configuration["CosmosDb:ConnectionString"]
            ?? throw new InvalidOperationException(
                "CosmosDb:ConnectionString is not configured.");

        var databaseName =
            _configuration["CosmosDb:DatabaseName"]
            ?? throw new InvalidOperationException(
                "CosmosDb:DatabaseName is not configured.");

        var containerName =
            _configuration["CosmosDb:ContainerName"]
            ?? throw new InvalidOperationException(
                "CosmosDb:ContainerName is not configured.");

        using var cosmosClient = new CosmosClient(connectionString);

        var container = cosmosClient.GetContainer(
            databaseName,
            containerName);

        await container.UpsertItemAsync(
            result,
            cancellationToken: cancellationToken);
    }
}