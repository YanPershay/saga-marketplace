import { gatewayRequest } from "./client";
import type { Product } from "../types/product";

export function getProducts() {
  return gatewayRequest<Product[]>("/api/catalog/products");
}
