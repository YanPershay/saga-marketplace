# Saga Marketplace Frontend

React + TypeScript technical demo UI for the Saga Marketplace distributed backend. The frontend talks only to the Gateway API and presents the main marketplace flow: catalog, product details, AI recommendations with fallback, cart, checkout, and live order saga tracking.

## Run Locally

```bash
cd frontend
npm install
npm run dev -- --host 127.0.0.1
```

Open:

```text
http://127.0.0.1:5173/
```

Production build and lint:

```bash
npm run build
npm run lint
```

## Environment

Create `frontend/.env` from `frontend/.env.example`:

```env
VITE_API_BASE_URL=https://your-gateway.example.com
```

The app reads API configuration from `VITE_API_BASE_URL`. It does not call backend services directly and does not use localhost as an API fallback.

## Implemented Flows

- Catalog page at `/products` with live products from the Gateway.
- Product details page at `/products/:productId`.
- AI recommendations request and polling through the Gateway.
- Recommendation fallback cards when AI is unavailable, times out, or returns no recommendations.
- Frontend-only cart state with Zustand.
- Checkout/create order flow from cart items.
- Redirect to `/orders/:orderId` after successful order creation.
- Live order status page with 2 second polling.
- Saga timeline for known statuses, with terminal handling for `Completed`, `Failed`, and `Cancelled`.

## Gateway Endpoints

All requests are built from `VITE_API_BASE_URL`:

```text
GET  /api/catalog/products
POST /api/catalog/products/{productId}/recommendations
GET  /api/catalog/products/{productId}/recommendations/{requestId}
POST /api/orders
GET  /api/orders/{orderId}
```

The frontend does not call `catalog-api`, `order-api`, `ai-api`, Kubernetes service names, databases, queues, or internal worker endpoints.

## UX Notes

- Catalog `quantityAvailable` is not displayed as exact stock. Inventory reservation is treated as the checkout-time source of truth.
- Product cards show neutral availability text: `Stock checked during checkout`.
- AI recommendation cards are labeled `AI recommended`.
- Fallback recommendation cards are labeled `Recommended fallback`.
- Failed and cancelled order states use terminal visual treatment and do not imply successful future saga steps.

## Smoke Test Checklist

1. Open `/products` and verify the live catalog grid renders.
2. Open a product details page with the `Details` button.
3. Click `Generate AI recommendations` and verify either AI recommendations or fallback recommendations appear.
4. Add a product to the cart from catalog, details, or recommendations.
5. Open `/cart`, adjust quantities, and verify totals update.
6. Click `Create order`.
7. Confirm redirect to `/orders/{orderId}`.
8. Observe the saga timeline polling until a terminal status.
9. If available, verify `Failed` or `Cancelled` orders show terminal styling without marking future steps successful.
