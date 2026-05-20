import { gatewayRequest } from "./client";
import type {
  RecommendationRequest,
  RecommendationRequestResponse,
  RecommendationResult,
} from "../types/recommendation";

export function requestRecommendations(
  productId: string,
  request: RecommendationRequest,
) {
  return gatewayRequest<RecommendationRequestResponse>(
    `/api/catalog/products/${productId}/recommendations`,
    {
      method: "POST",
      body: request,
    },
  );
}

export function getRecommendationResult(productId: string, requestId: string) {
  return gatewayRequest<RecommendationResult>(
    `/api/catalog/products/${productId}/recommendations/${requestId}`,
  );
}
