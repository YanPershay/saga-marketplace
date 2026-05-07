using BuildingBlocks.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Persistence.Outbox;

public sealed class OutboxPublisher
{
    private const int BatchSize = 20;
    private const int MaxRetryCount = 3;

    private readonly InventoryDbContext _context;
    private readonly IRawMessagePublisher _publisher;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(InventoryDbContext context, IRawMessagePublisher publisher, ILogger<OutboxPublisher> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task PublishPendingMessageAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _context.OutboxMessages
            .Where(x => x.Status == OutboxMessageStatus.Pending)
            .OrderBy(x => x.OccurredAt)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
            return;

        _logger.LogInformation(
            "Found {OutboxMessageCount} pending outbox messages.",
            messages.Count);

        foreach (var message in messages)
        {
            try
            {
                await _publisher.PublishAsync(
                    message.RoutingKey,
                    message.Payload,
                    cancellationToken);

                message.MarkAsSent();

                _logger.LogInformation(
                    "Outbox message {MessageId} published successfully.",
                    message.MessageId);
            }
            catch (Exception ex)
            {
                var error = ex.Message;

                if (message.RetryCount + 1 >= MaxRetryCount)
                {
                    message.MarkAsFailed(error);

                    _logger.LogError(
                        ex,
                        "Outbox message {MessageId} failed after {RetryCount} retries.",
                        message.MessageId,
                        message.RetryCount);
                }
                else
                {
                    message.IncreaseRetry(error);

                    _logger.LogWarning(
                        ex,
                        "Failed to publish outbox message {MessageId}. RetryCount: {RetryCount}.",
                        message.MessageId,
                        message.RetryCount);
                }
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}