using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using Inventory.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Inventory;

public sealed class InventoryCommitRequestedHandler
{
    private const string ConsumerName = "inventory-commit-requested-consumer";

    private readonly IInventoryUnitOfWork _unitOfWork;
    private readonly ILogger<InventoryCommitRequestedHandler> _logger;

    public InventoryCommitRequestedHandler(
        IInventoryUnitOfWork unitOfWork,
        ILogger<InventoryCommitRequestedHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(
        EventEnvelope<InventoryCommitRequestedIntegrationEvent> envelope,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed = await _unitOfWork.HasInboxMessageAsync(
            envelope.MessageId,
            ConsumerName,
            cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogWarning(
                "Duplicate InventoryCommitRequested event detected. MessageId: {MessageId}",
                envelope.MessageId);

            return;
        }

        _logger.LogInformation(
            "Handling InventoryCommitRequested event for OrderId: {OrderId}",
            envelope.Payload.OrderId);

        await _unitOfWork.CommitReservationAndSaveInboxAsync(
            envelope,
            ConsumerName,
            cancellationToken);

        _logger.LogInformation(
            "InventoryCommitRequested handled successfully for OrderId: {OrderId}",
            envelope.Payload.OrderId);
    }
}