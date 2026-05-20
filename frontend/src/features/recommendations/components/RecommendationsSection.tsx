import { RecommendedProductCard } from "./RecommendedProductCard";
import { useRecommendationsFlow } from "../hooks/useRecommendationsFlow";
import { getFallbackProducts } from "../lib/recommendationProducts";
import type { Product } from "../../../types/product";

type RecommendationsSectionProps = {
  currentProduct: Product;
  products: Product[];
};

const FALLBACK_REASON =
  "AI recommendations are temporarily unavailable. Showing similar products from the catalog.";

export function RecommendationsSection({
  currentProduct,
  products,
}: RecommendationsSectionProps) {
  const recommendationFlow = useRecommendationsFlow(currentProduct, products);
  const fallbackProducts = getFallbackProducts(products, currentProduct.id);
  const completedRecommendations =
    recommendationFlow.result?.status === "Completed"
      ? (recommendationFlow.result.recommendations ?? [])
      : [];
  const aiRecommendedProducts = completedRecommendations
    .map((recommendation) => ({
      recommendation,
      product: products.find(
        (product) => product.id === recommendation.productId,
      ),
    }))
    .filter(
      (entry): entry is NonNullable<typeof entry> & { product: Product } =>
        Boolean(entry.product && entry.product.id !== currentProduct.id),
    );
  const shouldShowFallback =
    recommendationFlow.status === "failed" ||
    recommendationFlow.status === "timeout" ||
    (recommendationFlow.status === "completed" &&
      aiRecommendedProducts.length === 0);

  return (
    <section className="rounded-3xl border border-violet-400/25 bg-[rgba(20,20,32,0.75)] p-6 shadow-neon-soft backdrop-blur-xl">
      <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.24em] text-neon-glow">
            Recommendations
          </p>
          <h3 className="mt-2 text-2xl font-semibold text-slate-50">
            AI pairing engine
          </h3>
          <p className="mt-2 max-w-2xl text-sm leading-6 text-zinc-400">
            Ask the gateway for AI recommendations. If the provider is
            unavailable, the page keeps working with catalog fallback picks.
          </p>
        </div>

        <button
          type="button"
          onClick={() => recommendationFlow.generate()}
          disabled={recommendationFlow.isBusy || products.length <= 1}
          className="rounded-2xl border border-violet-300/35 bg-violet-500/20 px-5 py-3 text-sm font-semibold text-white transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/70 hover:bg-violet-500/30 hover:shadow-neon disabled:cursor-not-allowed disabled:opacity-60 disabled:hover:translate-y-0 disabled:hover:shadow-none"
        >
          {recommendationFlow.isBusy
            ? "Generating..."
            : recommendationFlow.status === "failed" ||
                recommendationFlow.status === "timeout"
              ? "Retry AI recommendations"
              : "Generate AI recommendations"}
        </button>
      </div>

      <RecommendationStatusPanel
        status={recommendationFlow.status}
        error={recommendationFlow.error}
        requestId={recommendationFlow.requestId}
      />

      {products.length <= 1 ? (
        <RecommendationEmptyState />
      ) : recommendationFlow.status === "completed" &&
        aiRecommendedProducts.length > 0 ? (
        <div className="mt-6 grid gap-5 md:grid-cols-2">
          {aiRecommendedProducts.map(({ product, recommendation }) => (
            <RecommendedProductCard
              key={product.id}
              product={product}
              label="AI recommended"
              reason={recommendation.reason}
            />
          ))}
        </div>
      ) : shouldShowFallback ? (
        <div className="mt-6 grid gap-5 md:grid-cols-2">
          {fallbackProducts.map((product) => (
            <RecommendedProductCard
              key={product.id}
              product={product}
              label="Recommended fallback"
              reason={FALLBACK_REASON}
            />
          ))}
        </div>
      ) : (
        <div className="mt-6 rounded-2xl border border-dashed border-violet-400/25 bg-black/20 p-6 text-sm leading-6 text-zinc-400">
          Generate recommendations to see AI matches for this product.
        </div>
      )}
    </section>
  );
}

function RecommendationStatusPanel({
  status,
  error,
  requestId,
}: {
  status: ReturnType<typeof useRecommendationsFlow>["status"];
  error: Error | null;
  requestId?: string;
}) {
  if (status === "idle") return null;

  if (status === "requesting" || status === "polling") {
    return (
      <div className="mt-5 rounded-2xl border border-violet-300/25 bg-violet-500/10 p-4">
        <p className="text-sm font-semibold text-neon-glow">
          {status === "requesting"
            ? "Requesting AI recommendations..."
            : "Polling recommendation result every 2 seconds..."}
        </p>
        {requestId ? (
          <p className="mt-2 break-all font-mono text-xs text-zinc-400">
            requestId: {requestId}
          </p>
        ) : null}
      </div>
    );
  }

  if (status === "failed" || status === "timeout") {
    return (
      <div className="mt-5 rounded-2xl border border-amber-300/25 bg-amber-300/10 p-4">
        <p className="text-sm font-semibold text-neon-warning">
          {status === "timeout"
            ? "AI recommendation timeout"
            : "AI recommendations unavailable"}
        </p>
        <p className="mt-2 text-sm leading-6 text-zinc-300">
          {error?.message ?? FALLBACK_REASON}
        </p>
      </div>
    );
  }

  return null;
}

function RecommendationEmptyState() {
  return (
    <div className="mt-6 rounded-2xl border border-dashed border-violet-400/25 bg-black/20 p-6 text-sm leading-6 text-zinc-400">
      There are no other catalog products to recommend yet.
    </div>
  );
}
