# Saga Marketplace Frontend Requirements

## Goal

Build a React frontend for Saga Marketplace.

This frontend is a technical demo for a distributed marketplace backend with:

- product catalog
- AI recommendations
- cart
- checkout
- order saga status tracking

The goal is not to build a full ecommerce platform.

The goal is to clearly demonstrate the backend flows through a clean UI.

---

## Gateway

Frontend must use only the Gateway.

Base URL:

```text
http://20.223.61.242
```

In frontend code use:
```
VITE_API_BASE_URL=http://20.223.61.242
```
---

## Visual Design Direction

The frontend should look like a modern dark neon gaming/computer club marketplace.

Style keywords:

- dark cyber marketplace
- premium gaming hardware store
- neon purple accents
- black / near-black background
- violet / purple gradients
- glassmorphism cards
- subtle glow effects
- smooth hover animations
- clean futuristic UI
- not childish, not overly colorful

Main color direction:

```text
Background: #050509, #0B0B12, #11111A
Surface/Card: rgba(20, 20, 32, 0.75)
Primary Accent: #8B5CF6
Secondary Accent: #A855F7
Neon Accent: #C084FC
Text Primary: #F8FAFC
Text Secondary: #A1A1AA
Border: rgba(139, 92, 246, 0.25)
Error: #F87171
Success: #34D399
Warning: #FBBF24
```

UI style:

* dark layout
* purple neon highlights
* soft shadows
* rounded cards
* subtle animated glow
* hover lift effect on product cards
* gradient buttons
* clean spacing
* responsive grid
* no default browser-looking UI

Animations:

* subtle transitions only
* hover scale/lift for cards
* loading skeleton shimmer
* smooth status timeline transitions
* avoid excessive distracting animations

Product images:

Backend currently may not provide image URLs.

If product image is missing, use generated frontend placeholders based on product category/name.

Do not fetch external random images.

Use local deterministic placeholder components, for example:

* gradient card background
* abstract device icon
* initials/category label
* neon frame

For computer hardware products, placeholders can visually represent:

* keyboard
* mouse
* monitor
* processor
* SSD/storage
* accessories

The placeholder should be generated in UI code with CSS/React, not by downloading images.

---
Pages

# 1. Catalog Page

Route:
```
/products
```

Features:

* load products
* show product cards
* show name, description, price, category
* add to cart
* generate recommendations
* loading state
* error state
* empty state

# 2. Cart / Checkout page

Route:
```
/cart
```

Features:

* show cart items
* change quantity
* remove item
* show total
* create order
* navigate to order status page after order creation

# 3. Order Status Page

Route:
```
/orders/:orderId
```

Features:

* fetch order by id
* poll order status
* show saga timeline
* stop polling on terminal status

Terminal statuses:
```
Completed
Failed
Cancelled
```

Known statuses:
```
Pending
AwaitingInventory
InventoryReserved
PaymentProcessed
ShippingScheduled
Completed
Failed
Cancelled
```

Current live response example:
```
{
  "id": "b219f8e1-f084-45f0-a06a-f875b48be613",
  "customerId": "c3d8c4b2-0a0d-4f9b-bc58-6d2b4d2f10ab",
  "status": "AwaitingInventory",
  "totalPrice": 0.00,
  "createdAt": "2026-05-20T15:09:07.8935+00:00",
  "items": [
    {
      "productId": "faeaff02-7e51-4397-a928-fa66ee824ec8",
      "quantity": 1,
      "price": 0.00
    }
  ]
}
```

---
 # API Contracts

Get Products

```
GET /api/catalog/products
```

Full URL:
```
${VITE_API_BASE_URL}/api/catalog/products
```

Expected response:
```
[
  {
    "id": "faeaff02-7e51-4397-a928-fa66ee824ec8",
    "name": "Mechanical Keyboard Keychron K8",
    "description": "Wireless mechanical keyboard with hot-swappable switches and RGB backlight.",
    "price": 129.99,
    "quantityAvailable": 25,
    "createdAt": "2026-05-20T...",
    "category": "Keyboards"
  }
]
```
Note: exact product fields should be based on live backend response.

⸻

Create Recommendation Request

POST /api/catalog/products/{productId}/recommendations

Request body:
```
{
  "currentProduct": {
    "id": "faeaff02-7e51-4397-a928-fa66ee824ec8",
    "name": "Mechanical Keyboard Keychron K8",
    "description": "Wireless mechanical keyboard with hot-swappable switches and RGB backlight.",
    "price": 129.99,
    "category": "Keyboards"
  },
  "candidateProducts": [
    {
      "id": "b2137441-2848-402c-a533-e678696c529c",
      "name": "Logitech MX Master 3S",
      "description": "Advanced wireless productivity mouse with silent clicks and USB-C charging.",
      "price": 99.50,
      "category": "Accessories"
    }
  ]
}
```

Response:
```
{
  "requestId": "f00e7b22-dd9b-4c81-8126-7f1b04b7c0c3",
  "status": "Processing"
}
```
⸻

Get Recommendation Result
```
GET /api/catalog/products/{productId}/recommendations/{requestId}
```
Processing response:
```
{
  "requestId": "f00e7b22-dd9b-4c81-8126-7f1b04b7c0c3",
  "status": "Processing"
}
```
Completed response:
```
{
  "id": "f00e7b22-dd9b-4c81-8126-7f1b04b7c0c3",
  "requestId": "f00e7b22-dd9b-4c81-8126-7f1b04b7c0c3",
  "productId": "faeaff02-7e51-4397-a928-fa66ee824ec8",
  "recommendations": [
    {
      "productId": "b2137441-2848-402c-a533-e678696c529c",
      "reason": "Recommended because it complements the selected product."
    }
  ],
  "provider": "Gemini",
  "model": "gemini-2.5-flash",
  "status": "Completed",
  "generatedAtUtc": "2026-05-20T..."
}
```
Failed response:
```
{
  "requestId": "f00e7b22-dd9b-4c81-8126-7f1b04b7c0c3",
  "status": "Failed",
  "errorMessage": "Gemini service temporarily unavailable."
}
```
⸻

Create Order
```
POST /api/orders
```
Request body:
```
{
  "customerId": "c3d8c4b2-0a0d-4f9b-bc58-6d2b4d2f10ab",
  "items": [
    {
      "productId": "faeaff02-7e51-4397-a928-fa66ee824ec8",
      "quantity": 1,
      "unitPrice": 129.99
    }
  ]
}
```
Current live response:
```
{
  "orderId": "b219f8e1-f084-45f0-a06a-f875b48be613",
  "status": "Pending"
}
```
⸻

Get Order By Id
```
GET /api/orders/{orderId}
```
Current live response:
```
{
  "id": "b219f8e1-f084-45f0-a06a-f875b48be613",
  "customerId": "c3d8c4b2-0a0d-4f9b-bc58-6d2b4d2f10ab",
  "status": "AwaitingInventory",
  "totalPrice": 0.00,
  "createdAt": "2026-05-20T15:09:07.8935+00:00",
  "items": [
    {
      "productId": "faeaff02-7e51-4397-a928-fa66ee824ec8",
      "quantity": 1,
      "price": 0.00
    }
  ]
}
```
⸻

# Polling

Recommendation polling

Poll every:
```
2 seconds
```
Stop on:
```
Completed
Failed
```
Timeout after:
```
60 seconds
```
⸻

Order polling

Poll every:
```
2 seconds
```
Stop on:
```
Completed
Failed
Cancelled
```
⸻

Cart

Cart is frontend-only state.

Use Zustand.

Cart item shape:
```
type CartItem = {
  productId: string;
  name: string;
  price: number;
  quantity: number;
};
```
⸻

UI Requirements

Must include:

* clean layout
* product cards
* cart summary
* order timeline
* recommendation cards
* status badges
* loading skeletons or loaders
* retry buttons
* empty states

⸻

Recommended MVP Flow

1. Open catalog
2. Load products
3. Add product to cart
4. Generate AI recommendations for product
5. Add recommended product to cart
6. Open cart
7. Create order
8. Navigate to order status
9. Poll until terminal status

⸻

Do Not Build

* authentication
* payment UI
* user profiles
* admin panel
* product management
* complex filters
* SSR
* Next.js
* fake backend


