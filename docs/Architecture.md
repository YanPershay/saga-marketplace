# System Architecture

## Overview

The system is a mini-marketplace built using an event-driven microservices architecture.
Each business capability is isolated into its own service with strict boundaries.

---

## Communication Model

### HTTP
- Frontend → API Gateway
- API Gateway → services (read-only queries)

### Events
- All business workflows are event-driven
- RabbitMQ is used as the message broker
- No direct HTTP communication between business services

---

## Services Responsibilities

### Catalog
- Product catalog
- Read-optimized data for frontend

### Order
- Owns order lifecycle
- Implements Saga orchestration
- Maintains Saga state

### Inventory
- Manages product stock
- Reserves and releases inventory

### Payment
- Processes payments
- Emits payment success / failure events

### Shipping
- Arranges delivery
- Emits shipment status events

### Notification
- Sends emails / notifications
- No impact on business flow

### AI Service
- Asynchronous AI jobs
- Non-blocking
- Communicates via events

---

## API Gateway

- Single entry point for frontend
- Request routing
- Read-model aggregation
- No business logic
- No orchestration logic