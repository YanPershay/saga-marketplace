import { useMutation, useQuery } from "@tanstack/react-query";
import { useMemo, useState } from "react";
import {
  getRecommendationResult,
  requestRecommendations,
} from "../../../api/recommendationsApi";
import { queryKeys } from "../../../lib/queryKeys";
import type { Product } from "../../../types/product";
import type { RecommendationResult } from "../../../types/recommendation";
import { toRecommendationProduct } from "../lib/recommendationProducts";

const RECOMMENDATION_TIMEOUT_MS = 60_000;

export type RecommendationFlowStatus =
  | "idle"
  | "requesting"
  | "polling"
  | "completed"
  | "failed"
  | "timeout";

export function useRecommendationsFlow(
  currentProduct: Product | undefined,
  products: Product[],
) {
  const [requestId, setRequestId] = useState<string>();
  const [startedAt, setStartedAt] = useState<number>();
  const [timedOutRequestId, setTimedOutRequestId] = useState<string>();

  const mutation = useMutation({
    mutationFn: () => {
      if (!currentProduct) {
        throw new Error("Product is not available for recommendations.");
      }

      return requestRecommendations(currentProduct.id, {
        currentProduct: toRecommendationProduct(currentProduct),
        candidateProducts: products
          .filter((product) => product.id !== currentProduct.id)
          .map(toRecommendationProduct),
      });
    },
    onMutate: () => {
      setRequestId(undefined);
      setTimedOutRequestId(undefined);
      setStartedAt(Date.now());
    },
    onSuccess: (response) => {
      setRequestId(response.requestId);
    },
  });

  const resultQuery = useQuery({
    queryKey: queryKeys.recommendations.result(
      currentProduct?.id ?? "",
      requestId ?? "",
    ),
    queryFn: () => getRecommendationResult(currentProduct!.id, requestId!),
    enabled: Boolean(currentProduct && requestId),
    refetchInterval: (query) => {
      const result = query.state.data as RecommendationResult | undefined;

      if (result?.status === "Completed" || result?.status === "Failed") {
        return false;
      }

      if (startedAt && Date.now() - startedAt >= RECOMMENDATION_TIMEOUT_MS) {
        if (requestId) setTimedOutRequestId(requestId);
        return false;
      }

      return 2000;
    },
  });

  const status: RecommendationFlowStatus = useMemo(() => {
    if (timedOutRequestId && timedOutRequestId === requestId) return "timeout";
    if (mutation.isPending) return "requesting";
    if (mutation.isError || resultQuery.isError) return "failed";
    if (resultQuery.data?.status === "Failed") return "failed";
    if (resultQuery.data?.status === "Completed") return "completed";
    if (requestId) return "polling";
    return "idle";
  }, [
    mutation.isError,
    mutation.isPending,
    requestId,
    resultQuery.data?.status,
    resultQuery.isError,
    timedOutRequestId,
  ]);

  return {
    generate: mutation.mutate,
    status,
    requestId,
    result: resultQuery.data,
    error: mutation.error ?? resultQuery.error,
    isBusy: status === "requesting" || status === "polling",
  };
}
