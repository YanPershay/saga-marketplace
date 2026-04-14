# Observability

## Logging
- Structured logs
- CorrelationId included in all logs

## Tracing
- Distributed tracing via OpenTelemetry
- CorrelationId propagated through message headers

## Metrics
- Retry count
- DLQ size
- Outbox backlog
- Saga duration
- Failure rate per Saga step