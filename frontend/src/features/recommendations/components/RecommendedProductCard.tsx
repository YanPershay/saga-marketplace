import { Link } from "react-router-dom";
import { ProductVisualPlaceholder } from "../../catalog/components/ProductVisualPlaceholder";
import { formatMoney } from "../../../lib/money";
import { useCartStore } from "../../../stores/cartStore";
import type { Product } from "../../../types/product";

type RecommendedProductCardProps = {
  product: Product;
  label: "AI recommended" | "Recommended fallback";
  reason: string;
};

export function RecommendedProductCard({
  product,
  label,
  reason,
}: RecommendedProductCardProps) {
  const addProduct = useCartStore((state) => state.addProduct);

  return (
    <article className="flex h-full flex-col overflow-hidden rounded-3xl border border-violet-400/25 bg-[rgba(20,20,32,0.75)] p-4 shadow-neon-soft backdrop-blur-xl transition duration-300 hover:-translate-y-1 hover:border-neon-glow/55 hover:shadow-neon">
      {product.imageUrl ? (
        <img
          src={product.imageUrl}
          alt={product.name}
          className="aspect-[4/3] rounded-2xl border border-violet-400/25 object-cover"
        />
      ) : (
        <ProductVisualPlaceholder product={product} />
      )}

      <div className="flex flex-1 flex-col pt-5">
        <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
          <span
            className={[
              "rounded-full border px-3 py-1 text-xs font-semibold uppercase tracking-[0.16em]",
              label === "AI recommended"
                ? "border-neon-glow/40 bg-violet-500/20 text-neon-glow"
                : "border-amber-300/30 bg-amber-300/10 text-neon-warning",
            ].join(" ")}
          >
            {label}
          </span>
          <span className="rounded-2xl border border-emerald-300/20 bg-emerald-400/10 px-3 py-1 text-sm font-semibold text-neon-success">
            {formatMoney(product.price)}
          </span>
        </div>

        <h3 className="text-lg font-semibold leading-6 text-slate-50">
          {product.name}
        </h3>
        <p className="mt-2 line-clamp-3 text-sm leading-6 text-zinc-400">
          {reason}
        </p>

        <div className="mt-5 grid gap-2 sm:grid-cols-2">
          <button
            type="button"
            onClick={() => addProduct(product)}
            className="rounded-2xl border border-violet-300/35 bg-violet-500/15 px-4 py-3 text-sm font-semibold text-white transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/70 hover:bg-violet-500/25 hover:shadow-neon"
          >
            Add to cart
          </button>
          <Link
            to={`/products/${product.id}`}
            className="rounded-2xl border border-violet-400/20 bg-black/25 px-4 py-3 text-center text-sm font-semibold text-zinc-200 transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/60 hover:bg-violet-500/10 hover:text-white"
          >
            Details
          </Link>
        </div>
      </div>
    </article>
  );
}
