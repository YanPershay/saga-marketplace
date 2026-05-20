import { gatewayRequest } from "./client";
import type {
  CreateOrderRequest,
  CreateOrderResponse,
  Order,
} from "../types/order";

export function createOrder(request: CreateOrderRequest) {
  return gatewayRequest<CreateOrderResponse>("/api/orders", {
    method: "POST",
    body: request,
  });
}

export function getOrder(orderId: string) {
  return gatewayRequest<Order>(`/api/orders/${orderId}`);
}
