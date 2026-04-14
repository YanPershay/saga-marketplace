# Architectural Rules

## Service Boundaries
- Each service owns its database
- No direct access to another service’s data
- No shared schemas

## Communication
- HTTP is forbidden between business services
- All business communication is event-based

## Saga
- Orchestration-based
- Orchestrator: Order Service
- State stored in Order database
- Compensation via events only

## Reliability
- Outbox pattern is mandatory
- Inbox / deduplication is mandatory
- All event handlers must be idempotent
- Retry for transient failures only
- Poison messages go to DLQ

## Configuration
- Environment-based configuration only
- No hard-coded values
- Ready for Kubernetes deployment