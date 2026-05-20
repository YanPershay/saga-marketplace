import { PageHeader } from "../components/ui/PageHeader";
import { ProductCard } from "../features/catalog/components/ProductCard";
import { ProductGridSkeleton } from "../features/catalog/components/ProductGridSkeleton";
import { useProductsQuery } from "../features/catalog/hooks/useProductsQuery";

export function ProductsPage() {
  const { data: products, isLoading, isError, error, refetch } = useProductsQuery();

  return (
    <div>
      <PageHeader
        eyebrow="Catalog"
        title="Products arena"
        description="Live gateway catalog with resilient product cards, cart actions, and detail pages for AI-assisted discovery."
      />

      {isLoading ? <ProductGridSkeleton /> : null}

      {isError ? (
        <CatalogErrorState
          message={error instanceof Error ? error.message : "Unable to load products."}
          onRetry={() => void refetch()}
        />
      ) : null}

      {!isLoading && !isError && products?.length === 0 ? (
        <CatalogEmptyState />
      ) : null}

      {!isLoading && !isError && products && products.length > 0 ? (
        <div className="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
          {products.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      ) : null}
    </div>
  );
}

function CatalogErrorState({
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
        Could not load products
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

function CatalogEmptyState() {
  return (
    <div className="rounded-3xl border border-violet-400/25 bg-[rgba(20,20,32,0.75)] p-8 text-center shadow-neon-soft backdrop-blur-xl">
      <p className="text-sm font-semibold uppercase tracking-[0.24em] text-neon-glow">
        Empty catalog
      </p>
      <h3 className="mt-3 text-2xl font-semibold text-slate-50">
        No products are available
      </h3>
      <p className="mx-auto mt-3 max-w-xl text-sm leading-6 text-zinc-400">
        The gateway responded successfully, but the catalog returned an empty
        collection.
      </p>
    </div>
  );
}
