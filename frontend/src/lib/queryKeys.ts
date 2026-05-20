export const queryKeys = {
  products: {
    all: ["products"] as const,
  },
  orders: {
    detail: (orderId: string) => ["orders", orderId] as const,
  },
  recommendations: {
    result: (productId: string, requestId: string) =>
      ["recommendations", productId, requestId] as const,
  },
} as const;
