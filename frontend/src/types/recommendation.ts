import type { Product } from "./product";

export type RecommendationProduct = Pick<
  Product,
  "id" | "name" | "description" | "price"
> & {
  category: string;
};

export type RecommendationRequest = {
  currentProduct: RecommendationProduct;
  candidateProducts: RecommendationProduct[];
};

export type RecommendationStatus = "Processing" | "Completed" | "Failed";

export type RecommendationRequestResponse = {
  requestId: string;
  status: RecommendationStatus;
};

export type RecommendationItem = {
  productId: string;
  reason: string;
};

export type RecommendationResult = {
  id?: string;
  requestId: string;
  productId?: string;
  recommendations?: RecommendationItem[];
  provider?: string;
  model?: string;
  status: RecommendationStatus;
  generatedAtUtc?: string;
  correlationId?: string;
  errorMessage?: string;
};
