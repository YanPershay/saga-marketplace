# AGENTS.md — Saga Marketplace Frontend

## Role

You are working as a frontend implementation agent inside the `frontend/` folder of the Saga Marketplace monorepo.

The human developer is the tech lead. You are an implementation assistant.

Do not make broad architectural decisions without asking.

---

## Project Goal

Build a clean, production-style React frontend for Saga Marketplace.

This is a technical demo frontend for an existing distributed backend.

The frontend should demonstrate:

- product catalog
- AI recommendations
- cart
- checkout
- order creation
- order saga status polling
- loading/error/empty states

---

## Backend Access

Frontend must communicate ONLY through the Gateway API.

Default local environment variable:

```env
VITE_API_BASE_URL=http://20.223.61.242
```

Do not call internal Kubernetes services directly.

Forbidden:
```
http://catalog-api
http://order-api
http://ai-api
http://postgres
http://rabbitmq
```

Allowed:
```
${VITE_API_BASE_URL}/api/catalog/products
${VITE_API_BASE_URL}/api/orders
${VITE_API_BASE_URL}/api/catalog/products/{productId}/recommendations
```

Tech Stack

Use:

* React
* TypeScript
* Vite
* TanStack Query
* Zustand
* Tailwind CSS
* React Router

Do not use:

* Next.js
* SSR
* Redux
* heavy UI libraries
* mock backend
* fake API data unless explicitly requested

⸻

Working Directory

All frontend code must live inside:
```
frontend/
```

Do not modify:
```
services/
infra/
gateway/
*.sln
backend project files
k8s manifests
```
Unless explicitly requested.

---

Development Commands

Expected commands:
```
npm install
npm run dev
npm run build
npm run lint
```

If a command fails, fix the frontend code instead of bypassing the command.

⸻

Code Style

Use:

* small components
* typed API contracts
* clear folder structure
* explicit loading/error states
* readable names
* no overengineering

Prefer:
```
src/api/
src/types/
src/features/
src/pages/
src/components/
src/stores/
src/lib/
```
---

API Rules

Use a central API client.

Do not hardcode URLs inside components.

Use:
```
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;
```
All DTOs should be typed.

---

Async Rules

Recommendations are async:

1. POST recommendation request
2. receive requestId
3. poll GET result
4. stop when Completed or Failed

Orders are saga-based:

1. POST create order
2. receive orderId
3. poll GET order
4. stop when Completed, Failed, or Cancelled

---

UX Requirements

Every async flow must have:

* loading state
* error state
* retry action where reasonable
* empty state
* stable layout
* no flickering during polling

⸻

Implementation Workflow

Do not implement everything at once.

Work in small steps:

1. scaffold app
2. add API client/types
3. add catalog page
4. add cart
5. add order creation
6. add order status polling
7. add recommendations flow
8. polish UI

After each major step, run:

```
npm run build
```

---

Important

Do not invent backend endpoints.

If an endpoint is unclear, stop and ask.

Do not silently change API contracts.

Do not hide failed states.

This backend is distributed and eventually consistent. The frontend must reflect Processing, Completed, and Failed states clearly.


