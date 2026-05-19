using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Recommendation.Functions.Clients;
using Recommendation.Functions.Messages;
using Recommendation.Functions.Models;
using Recommendation.Functions.Storage;

namespace Recommendation.Functions;

public sealed class ProcessRecommendationRequestedFunction
{
    private const string UnknownProvider = "Unknown";
    private const string UnknownModel = "Unknown";
    private const string ProcessingStatus = "Processing";
    private const string CompletedStatus = "Completed";
    private const string FailedStatus = "Failed";

    private readonly ILogger<ProcessRecommendationRequestedFunction> _logger;
    private readonly AiApiClient _aiApiClient;
    private readonly BlobStorageClient _blobStorageClient;
    private readonly CosmosRecommendationRepository _cosmosRepository;

    public ProcessRecommendationRequestedFunction(
        ILogger<ProcessRecommendationRequestedFunction> logger,
        AiApiClient aiApiClient,
        BlobStorageClient blobStorageClient,
        CosmosRecommendationRepository cosmosRepository)
    {
        _logger = logger;
        _aiApiClient = aiApiClient;
        _blobStorageClient = blobStorageClient;
        _cosmosRepository = cosmosRepository;
    }

    [Function("ProcessRecommendationRequested")]
    public async Task RunAsync(
        [ServiceBusTrigger(
            "recommendation-requested",
            "recommendation-processor",
            Connection = "ServiceBusConnection")]
        string message,
        FunctionContext context)
    {
        var recommendationRequested = DeserializeMessage(message);

        _logger.LogInformation(
            "Processing recommendation request. RequestId: {RequestId}, ProductId: {ProductId}, CorrelationId: {CorrelationId}",
            recommendationRequested.RequestId,
            recommendationRequested.ProductId,
            recommendationRequested.CorrelationId);

        await SaveProcessingResultAsync(
            recommendationRequested,
            context.CancellationToken);

        try
        {
            var aiResponse = await _aiApiClient.GetRecommendationsAsync(
                recommendationRequested,
                context.CancellationToken);

            _logger.LogInformation(
                "AI recommendations received. RequestId: {RequestId}",
                recommendationRequested.RequestId);

            await _blobStorageClient.SaveResponseAsync(
                recommendationRequested.RequestId,
                aiResponse,
                context.CancellationToken);

            _logger.LogInformation(
                "Raw AI response saved to Blob Storage. RequestId: {RequestId}",
                recommendationRequested.RequestId);

            var aiRecommendationResponse = DeserializeAiResponse(aiResponse);

            var completedResult = CreateCompletedResult(
                recommendationRequested,
                aiRecommendationResponse);

            await _cosmosRepository.SaveAsync(
                completedResult,
                context.CancellationToken);

            _logger.LogInformation(
                "Recommendation result saved to Cosmos DB. RequestId: {RequestId}",
                recommendationRequested.RequestId);
        }
        catch (Exception exception)
        {
            await HandleFailureAsync(
                recommendationRequested,
                exception);
        }
    }

    private static RecommendationRequestedMessage DeserializeMessage(string message)
    {
        return JsonConvert.DeserializeObject<RecommendationRequestedMessage>(message)
            ?? throw new InvalidOperationException(
                "Failed to deserialize RecommendationRequestedMessage.");
    }

    private static AiRecommendationResponse DeserializeAiResponse(string aiResponse)
    {
        return JsonConvert.DeserializeObject<AiRecommendationResponse>(aiResponse)
            ?? throw new InvalidOperationException(
                "Failed to deserialize AI recommendation response.");
    }

    private async Task SaveProcessingResultAsync(
        RecommendationRequestedMessage recommendationRequested,
        CancellationToken cancellationToken)
    {
        var processingResult = CreateBaseResult(
            recommendationRequested,
            Array.Empty<RecommendedProductResult>(),
            UnknownProvider,
            UnknownModel,
            ProcessingStatus,
            errorMessage: null);

        await _cosmosRepository.SaveAsync(
            processingResult,
            cancellationToken);

        _logger.LogInformation(
            "Processing recommendation result saved to Cosmos DB. RequestId: {RequestId}",
            recommendationRequested.RequestId);
    }

    private RecommendationResult CreateCompletedResult(
        RecommendationRequestedMessage recommendationRequested,
        AiRecommendationResponse aiRecommendationResponse)
    {
        var recommendations = aiRecommendationResponse.Recommendations
            .Select(x => new RecommendedProductResult(
                x.ProductId.ToString(),
                x.Reason))
            .ToList();

        return CreateBaseResult(
            recommendationRequested,
            recommendations,
            aiRecommendationResponse.Provider,
            aiRecommendationResponse.Model,
            CompletedStatus,
            errorMessage: null);
    }

    private RecommendationResult CreateFailedResult(
        RecommendationRequestedMessage recommendationRequested,
        Exception exception)
    {
        return CreateBaseResult(
            recommendationRequested,
            Array.Empty<RecommendedProductResult>(),
            UnknownProvider,
            UnknownModel,
            FailedStatus,
            exception.Message);
    }

    private static RecommendationResult CreateBaseResult(
        RecommendationRequestedMessage recommendationRequested,
        IReadOnlyCollection<RecommendedProductResult> recommendations,
        string provider,
        string model,
        string status,
        string? errorMessage)
    {
        return new RecommendationResult(
            recommendationRequested.RequestId.ToString(),
            recommendationRequested.RequestId.ToString(),
            recommendationRequested.ProductId.ToString(),
            recommendations,
            provider,
            model,
            status,
            DateTimeOffset.UtcNow,
            recommendationRequested.CorrelationId,
            errorMessage);
    }

    private async Task HandleFailureAsync(
        RecommendationRequestedMessage recommendationRequested,
        Exception exception)
    {
        _logger.LogError(
            exception,
            "Recommendation processing failed. RequestId: {RequestId}, ProductId: {ProductId}, CorrelationId: {CorrelationId}",
            recommendationRequested.RequestId,
            recommendationRequested.ProductId,
            recommendationRequested.CorrelationId);

        var failedResult = CreateFailedResult(
            recommendationRequested,
            exception);

        await _cosmosRepository.SaveAsync(
            failedResult,
            CancellationToken.None);

        _logger.LogInformation(
            "Failed recommendation result saved to Cosmos DB. RequestId: {RequestId}",
            recommendationRequested.RequestId);
    }
}