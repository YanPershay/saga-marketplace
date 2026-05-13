using System.Diagnostics;

namespace BuildingBlocks.Observability;

public static class MessagingTelemetry
{
    public const string ActivitySourceName = "SagaMarketplace.Messaging";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}