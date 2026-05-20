import { Link } from "react-router-dom";
import { formatMoney } from "../../../lib/money";
import { useCartStore } from "../../../stores/cartStore";
import type { Product } from "../../../types/product";
import { ProductVisualPlaceholder } from "./ProductVisualPlaceholder";

type ProductCardProps = {
  product: Product;
};

export function ProductCard({ product }: ProductCardProps) {
  const addProduct = useCartStore((state) => state.addProduct);
  const quantityInCart = useCartStore(
    (state) =>
      state.items.find((item) => item.productId === product.id)?.quantity ?? 0,
  );

  return (
    <article className="group flex h-full flex-col overflow-hidden rounded-3xl border border-violet-400/25 bg-[rgba(20,20,32,0.75)] p-4 shadow-neon-soft backdrop-blur-xl transition duration-300 hover:-translate-y-1 hover:border-neon-glow/55 hover:shadow-neon">
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
        <div className="mb-4 flex items-start justify-between gap-3">
          <div>
            {product.category ? (
              <p className="mb-2 inline-flex rounded-full border border-violet-400/25 bg-violet-500/10 px-3 py-1 text-xs font-semibold uppercase tracking-[0.18em] text-neon-glow">
                {product.category}
              </p>
            ) : null}
            <h3 className="line-clamp-2 text-lg font-semibold leading-6 text-slate-50 transition duration-300 group-hover:text-white">
              {product.name}
            </h3>
          </div>
          <p className="shrink-0 rounded-2xl border border-emerald-300/20 bg-emerald-400/10 px-3 py-2 text-sm font-semibold text-neon-success">
            {formatMoney(product.price)}
          </p>
        </div>

        <p className="line-clamp-3 flex-1 text-sm leading-6 text-zinc-400">
          {product.description}
        </p>

        <div className="mt-5 border-t border-violet-400/15 pt-4">
          <div className="flex items-center justify-between">
            <span className="text-xs uppercase tracking-[0.22em] text-zinc-500">
              Availability
            </span>
            <span className="rounded-full border border-violet-400/20 bg-black/25 px-3 py-1 text-sm font-medium text-zinc-200">
              Stock checked during checkout
            </span>
          </div>

          <button
            type="button"
            onClick={() => addProduct(product)}
            className="mt-4 flex w-full items-center justify-center rounded-2xl border border-violet-300/35 bg-violet-500/15 px-4 py-3 text-sm font-semibold text-white transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/70 hover:bg-violet-500/25 hover:shadow-neon"
          >
            {quantityInCart > 0
              ? `Add one more (${quantityInCart} in cart)`
              : "Add to cart"}
          </button>
          <Link
            to={`/products/${product.id}`}
            className="mt-3 flex w-full items-center justify-center rounded-2xl border border-violet-400/20 bg-black/25 px-4 py-3 text-sm font-semibold text-zinc-200 transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/60 hover:bg-violet-500/10 hover:text-white"
          >
            Details
          </Link>
        </div>
      </div>
    </article>
  );
}
