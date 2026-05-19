using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Catalog.API.Messaging;

public sealed class RecommendationRequestedPublisher
{
    private readonly ServiceBusClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RecommendationRequestedPublisher> _logger;

    public RecommendationRequestedPublisher(
        ServiceBusClient client,
        IConfiguration configuration,
        ILogger<RecommendationRequestedPublisher> logger)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Guid> PublishAsync(
        ProductContextMessage currentProduct,
        IReadOnlyCollection<CandidateProductMessage> candidateProducts,
        string? correlationId,
        CancellationToken cancellationToken = default)
    {
        var requestId = Guid.NewGuid();

        var topicName = _configuration["ServiceBus:RecommendationTopicName"]
                        ?? throw new InvalidOperationException("ServiceBus:RecommendationTopicName is not configured.");

        var sender = _client.CreateSender(topicName);

        var payload = new RecommendationRequestedMessage(
            requestId,
            currentProduct.Id,
            currentProduct,
            candidateProducts,
            correlationId ?? requestId.ToString(),
            DateTimeOffset.UtcNow);

        var body = JsonSerializer.Serialize(payload);

        var message = new ServiceBusMessage(body)
        {
            MessageId = requestId.ToString(),
            CorrelationId = payload.CorrelationId,
            ContentType = "application/json",
            Subject = "RecommendationRequested"
        };

        await sender.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation(
            "Published RecommendationRequested message. RequestId: {RequestId}, ProductId: {ProductId}, CorrelationId: {CorrelationId}",
            requestId,
            currentProduct.Id,
            payload.CorrelationId);

        return requestId;
    }
}