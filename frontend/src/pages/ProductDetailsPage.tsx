import { Link, useParams } from "react-router-dom";
import { GlassPanel } from "../components/ui/GlassPanel";
import { PageHeader } from "../components/ui/PageHeader";
import { ProductVisualPlaceholder } from "../features/catalog/components/ProductVisualPlaceholder";
import { useProductsQuery } from "../features/catalog/hooks/useProductsQuery";
import { RecommendationsSection } from "../features/recommendations/components/RecommendationsSection";
import { formatMoney } from "../lib/money";
import { useCartStore } from "../stores/cartStore";
import type { Product } from "../types/product";

export function ProductDetailsPage() {
  const { productId } = useParams();
  const { data: products, isLoading, isError, error, refetch } = useProductsQuery();
  const product = products?.find((item) => item.id === productId);

  return (
    <div>
      <PageHeader
        eyebrow="Product details"
        title={product?.name ?? "Product signal"}
        description="Inspect one catalog item, then ask the gateway for resilient AI recommendations."
      />

      {isLoading ? <ProductDetailsSkeleton /> : null}

      {isError ? (
        <ProductDetailsErrorState
          message={error instanceof Error ? error.message : "Unable to load products."}
          onRetry={() => void refetch()}
        />
      ) : null}

      {!isLoading && !isError && products && !product ? (
        <ProductNotFoundState productId={productId} />
      ) : null}

      {product && products ? (
        <div className="space-y-6">
          <ProductDetailsPanel product={product} />
          <RecommendationsSection currentProduct={product} products={products} />
        </div>
      ) : null}
    </div>
  );
}

function ProductDetailsPanel({ product }: { product: Product }) {
  const addProduct = useCartStore((state) => state.addProduct);

  return (
    <GlassPanel>
      <div className="grid gap-6 lg:grid-cols-[minmax(18rem,26rem)_minmax(0,1fr)]">
        {product.imageUrl ? (
          <img
            src={product.imageUrl}
            alt={product.name}
            className="aspect-[4/3] w-full rounded-3xl border border-violet-400/25 object-cover"
          />
        ) : (
          <ProductVisualPlaceholder product={product} />
        )}

        <div className="flex min-w-0 flex-col justify-between">
          <div>
            <div className="flex flex-wrap items-center gap-3">
              {product.category ? (
                <span className="rounded-full border border-violet-400/25 bg-violet-500/10 px-3 py-1 text-xs font-semibold uppercase tracking-[0.18em] text-neon-glow">
                  {product.category}
                </span>
              ) : null}
              <span className="rounded-full border border-violet-400/20 bg-black/25 px-3 py-1 text-xs font-semibold uppercase tracking-[0.18em] text-zinc-300">
                Available for order
              </span>
            </div>

            <h2 className="mt-5 text-3xl font-semibold tracking-normal text-slate-50">
              {product.name}
            </h2>
            <p className="mt-4 max-w-3xl text-base leading-7 text-zinc-400">
              {product.description}
            </p>
          </div>

          <div className="mt-6 grid gap-4 sm:grid-cols-[1fr_auto] sm:items-end">
            <div>
              <p className="text-xs uppercase tracking-[0.24em] text-zinc-500">
                Price
              </p>
              <p className="mt-2 text-4xl font-semibold text-neon-success">
                {formatMoney(product.price)}
              </p>
            </div>
            <button
              type="button"
              onClick={() => addProduct(product)}
              className="rounded-2xl border border-violet-300/35 bg-violet-500/20 px-5 py-3 text-sm font-semibold text-white transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/70 hover:bg-violet-500/30 hover:shadow-neon"
            >
              Add to cart
            </button>
          </div>
        </div>
      </div>
    </GlassPanel>
  );
}

function ProductDetailsSkeleton() {
  return (
    <GlassPanel>
      <div className="relative grid gap-6 overflow-hidden lg:grid-cols-[minmax(18rem,26rem)_minmax(0,1fr)]">
        <div className="absolute inset-0 -translate-x-full animate-shimmer bg-gradient-to-r from-transparent via-white/10 to-transparent" />
        <div className="aspect-[4/3] rounded-3xl bg-violet-400/10" />
        <div>
          <div className="h-5 w-32 rounded-full bg-violet-400/15" />
          <div className="mt-5 h-8 w-4/5 rounded-full bg-zinc-300/10" />
          <div className="mt-4 h-4 w-full rounded-full bg-zinc-300/10" />
          <div className="mt-2 h-4 w-3/4 rounded-full bg-zinc-300/10" />
          <div className="mt-8 h-12 w-44 rounded-2xl bg-violet-400/10" />
        </div>
      </div>
    </GlassPanel>
  );
}

function ProductDetailsErrorState({
  message,
  onRetry,
}: {
  message: string;
  onRetry: () => void;
}) {
  return (
    <div className="rounded-3xl border border-red-300/25 bg-red-400/10 p-6 shadow-neon-soft backdrop-blur-xl">
      <p className="text-sm font-semibold uppercase tracking-[0.24em] text-neon-error">
        Catalog offline
      </p>
      <h3 className="mt-3 text-2xl font-semibold text-slate-50">
        Could not load product details
      </h3>
      <p className="mt-3 max-w-3xl text-sm leading-6 text-zinc-300">{message}</p>
      <button
        type="button"
        onClick={onRetry}
        className="mt-5 rounded-2xl border border-violet-300/35 bg-violet-500/15 px-4 py-2 text-sm font-semibold text-white transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/70 hover:bg-violet-500/25 hover:shadow-neon"
      >
        Retry catalog
      </button>
    </div>
  );
}

function ProductNotFoundState({ productId }: { productId?: string }) {
  return (
    <GlassPanel>
      <div className="rounded-2xl border border-dashed border-violet-400/25 bg-black/20 p-8 text-center">
        <p className="text-sm font-semibold uppercase tracking-[0.24em] text-neon-glow">
          Product not found
        </p>
        <h3 className="mt-3 text-2xl font-semibold text-slate-100">
          This catalog signal is missing
        </h3>
        {productId ? (
          <p className="mx-auto mt-3 max-w-xl break-all font-mono text-sm leading-6 text-zinc-400">
            {productId}
          </p>
        ) : null}
        <Link
          to="/products"
          className="mt-6 inline-flex rounded-2xl border border-violet-300/35 bg-violet-500/15 px-4 py-2 text-sm font-semibold text-white transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/70 hover:bg-violet-500/25 hover:shadow-neon"
        >
          Back to catalog
        </Link>
      </div>
    </GlassPanel>
  );
}
