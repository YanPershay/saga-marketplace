export const queryKeys = {
  products: {
    all: ["products"] as const,
  },
  orders: {
    detail: (orderId: string) => ["orders", orderId] as const,
  },
} as const;
