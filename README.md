# Mini-Marketplace (production-grade pet-project)

## Overview

This project is a **production-grade microservices playground** designed to gain deep, practical experience with:

- Event-driven architecture
- Saga (orchestration-based)
- Data consistency and reliability
- Outbox / Inbox patterns
- Observability
- Kubernetes deployment
- Clean, high-level code structure

---

## High-Level Architecture

- Frontend communicates only with API Gateway
- API Gateway routes requests and aggregates read models
- Business services communicate **only via events**
- Order Service orchestrates business processes using Saga
- RabbitMQ is used as the message broker
- Each service owns its data and database

---

## Services

- **Catalog** — product catalog and read models
- **Order** — Saga orchestrator and order lifecycle
- **Inventory** — stock management
- **Payment** — payment processing
- **Shipping** — shipment arrangement
- **Notification** — side-effect notifications
- **AI Service** — async AI jobs (non-blocking)

---

## AI Recommendations

Catalog exposes:
GET /products/{id}/recommendations

Flow:
1. Catalog loads the current product.
2. Catalog selects recommendation candidates from the same category.
3. Catalog calls AI Service via HTTP.
4. AI Service calls Gemini and returns structured recommendations.
5. Catalog maps recommended product IDs back to full product data.

If AI Service is unavailable, times out, or returns an invalid response, Catalog returns fallback recommendations from local catalog candidates.

---

## Key Architectural Principles

- No shared databases
- No HTTP calls between business services
- Event-driven communication only
- Saga with orchestration
- Outbox / Inbox for reliability
- Idempotent message handling
- Observability as a first-class concern

---

## Reliability

- Outbox pattern for all publishers
- Inbox / deduplication for all consumers
- Retry policies for transient failures
- DLQ for poison messages
- Eventual consistency across services

---

## Observability

- Structured logging
- CorrelationId propagated through all services
- Distributed tracing (OpenTelemetry)
- Metrics for retries, DLQ, Saga duration, failures

---

## Kubernetes

The system is designed to be deployed in Kubernetes with:

- Readiness / liveness probes
- Graceful shutdown
- Rolling updates
- Resource limits and requests
- ConfigMaps and Secrets

---

## Non-Goals

- No premature optimization
- No choreography-based Saga
- No shared libraries with business logic
- No hidden synchronous dependencies

---

## Trade-Offs

- Mono-repo for learning and visibility
- Orchestration-based Saga for clarity
- Explicit infrastructure configuration instead of abstractions