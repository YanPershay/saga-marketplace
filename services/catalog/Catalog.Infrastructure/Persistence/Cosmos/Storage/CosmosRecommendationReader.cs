using Catalog.API.Contracts.Responses;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Catalog.Infrastructure.Persistence.Cosmos.Storage;

public sealed class CosmosRecommendationReader
{
    private readonly IConfiguration _configuration;

    public CosmosRecommendationReader(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<RecommendationResultResponse?> GetAsync(
        string requestId,
        CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration["CosmosDb:ConnectionString"]
                               ?? throw new InvalidOperationException("CosmosDb:ConnectionString is not configured.");

        var databaseName = _configuration["CosmosDb:DatabaseName"]
                           ?? throw new InvalidOperationException("CosmosDb:DatabaseName is not configured.");

        var containerName = _configuration["CosmosDb:ContainerName"]
                            ?? throw new InvalidOperationException("CosmosDb:ContainerName is not configured.");

        using var cosmosClient = new CosmosClient(connectionString);

        var container = cosmosClient.GetContainer(databaseName, containerName);

        var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.requestId = @requestId")
            .WithParameter("@requestId", requestId);

        using var iterator = container.GetItemQueryIterator<CosmosRecommendationDocument>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(requestId)
            });

        if (!iterator.HasMoreResults)
            return null;

        var response = await iterator.ReadNextAsync(cancellationToken);

        var document = response.FirstOrDefault();

        if (document is null)
            return null;

        return new RecommendationResultResponse(
            document.RequestId,
            document.ProductId,
            document.Recommendations,
            document.Provider,
            document.Model,
            document.Status,
            document.GeneratedAtUtc);
    }

    private sealed record CosmosRecommendationDocument(
        [property: JsonProperty("id")] string Id,

        [property: JsonProperty("requestId")] string RequestId,

        [property: JsonProperty("productId")] string ProductId,

        [property: JsonProperty("recommendations")]
        IReadOnlyCollection<RecommendationItemResponse> Recommendations,

        [property: JsonProperty("provider")] string Provider,

        [property: JsonProperty("model")] string Model,

        [property: JsonProperty("status")] string Status,

        [property: JsonProperty("generatedAtUtc")]
        DateTimeOffset GeneratedAtUtc);
}