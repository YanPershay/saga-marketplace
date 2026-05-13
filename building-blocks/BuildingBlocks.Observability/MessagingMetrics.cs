using System.Diagnostics.Metrics;

namespace BuildingBlocks.Observability;

public static class MessagingMetrics
{
    public const string MeterName = "SagaMarketplace.Messaging";

    public static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> MessagesProcessed =
        Meter.CreateCounter<long>("messaging.messages.processed");

    public static readonly Counter<long> MessagesFailed =
        Meter.CreateCounter<long>("messaging.messages.failed");

    public static readonly Counter<long> MessagesRetried =
        Meter.CreateCounter<long>("messaging.messages.retried");

    public static readonly Counter<long> MessagesMovedToDlq =
        Meter.CreateCounter<long>("messaging.messages.moved_to_dlq");
}