export type CreateOrderItemRequest = {
  productId: string;
  quantity: number;
  price: number;
};

export type CreateOrderRequest = {
  customerId: string;
  items: CreateOrderItemRequest[];
};

export type CreateOrderResponse = {
  orderId: string;
  status: string;
};

export type OrderItem = {
  productId: string;
  quantity: number;
  price: number;
};

export type OrderStatus =
  | "Pending"
  | "AwaitingInventory"
  | "InventoryReserved"
  | "AwaitingPayment"
  | "PaymentProcessed"
  | "AwaitingShipment"
  | "ShippingScheduled"
  | "Completed"
  | "Failed"
  | "Cancelled"
  | (string & {});

export type Order = {
  id: string;
  customerId: string;
  status: OrderStatus;
  totalPrice: number;
  createdAt: string;
  items: OrderItem[];
};
