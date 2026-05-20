import { GlassPanel } from "../components/ui/GlassPanel";
import { PageHeader } from "../components/ui/PageHeader";
import { ProductVisualPlaceholder } from "../features/catalog/components/ProductVisualPlaceholder";
import { formatMoney } from "../lib/money";
import { useCartStore, type CartItem } from "../stores/cartStore";

export function CartPage() {
  const items = useCartStore((state) => state.items);
  const removeItem = useCartStore((state) => state.removeItem);
  const increaseQuantity = useCartStore((state) => state.increaseQuantity);
  const decreaseQuantity = useCartStore((state) => state.decreaseQuantity);
  const clearCart = useCartStore((state) => state.clearCart);
  const totalPrice = useCartStore((state) => state.getTotalPrice());
  const totalItems = useCartStore((state) => state.getTotalItems());

  return (
    <div>
      <PageHeader
        eyebrow="Checkout"
        title="Cart staging bay"
        description="Frontend-only cart state for shaping the checkout payload. Order creation stays offline for this step."
      />

      {items.length === 0 ? (
        <CartEmptyState />
      ) : (
        <div className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_22rem]">
          <div className="space-y-4">
            {items.map((item) => (
              <CartLineItem
                key={item.productId}
                item={item}
                onDecrease={() => decreaseQuantity(item.productId)}
                onIncrease={() => increaseQuantity(item.productId)}
                onRemove={() => removeItem(item.productId)}
              />
            ))}
          </div>

          <GlassPanel className="h-fit">
            <p className="text-xs font-semibold uppercase tracking-[0.24em] text-neon-glow">
              Cart summary
            </p>
            <div className="mt-5 space-y-4 border-b border-violet-400/15 pb-5">
              <SummaryRow label="Items" value={String(totalItems)} />
              <SummaryRow label="Subtotal" value={formatMoney(totalPrice)} />
            </div>
            <div className="mt-5 flex items-center justify-between">
              <span className="text-base font-semibold text-slate-100">
                Total
              </span>
              <span className="text-2xl font-semibold text-neon-success">
                {formatMoney(totalPrice)}
              </span>
            </div>
            <button
              type="button"
              className="mt-6 w-full rounded-2xl border border-violet-300/25 bg-violet-500/10 px-4 py-3 text-sm font-semibold text-zinc-300 opacity-75"
              disabled
            >
              Checkout coming next
            </button>
            <button
              type="button"
              onClick={clearCart}
              className="mt-3 w-full rounded-2xl border border-red-300/25 bg-red-400/10 px-4 py-3 text-sm font-semibold text-neon-error transition duration-200 hover:-translate-y-0.5 hover:border-red-300/45 hover:bg-red-400/15"
            >
              Clear cart
            </button>
          </GlassPanel>
        </div>
      )}
    </div>
  );
}

function CartLineItem({
  item,
  onDecrease,
  onIncrease,
  onRemove,
}: {
  item: CartItem;
  onDecrease: () => void;
  onIncrease: () => void;
  onRemove: () => void;
}) {
  const productLike = {
    id: item.productId,
    name: item.name,
    description: item.description,
    price: item.price,
    quantityAvailable: item.quantity,
    createdAt: "",
    category: item.category,
    imageUrl: item.imageUrl,
  };

  return (
    <article className="grid gap-4 rounded-3xl border border-violet-400/25 bg-[rgba(20,20,32,0.75)] p-4 shadow-neon-soft backdrop-blur-xl transition duration-300 hover:border-neon-glow/45 hover:shadow-neon md:grid-cols-[9rem_minmax(0,1fr)]">
      {item.imageUrl ? (
        <img
          src={item.imageUrl}
          alt={item.name}
          className="aspect-[4/3] w-full rounded-2xl border border-violet-400/25 object-cover md:aspect-square"
        />
      ) : (
        <ProductVisualPlaceholder product={productLike} />
      )}

      <div className="flex min-w-0 flex-col">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
          <div className="min-w-0">
            {item.category ? (
              <p className="mb-2 inline-flex rounded-full border border-violet-400/25 bg-violet-500/10 px-3 py-1 text-xs font-semibold uppercase tracking-[0.18em] text-neon-glow">
                {item.category}
              </p>
            ) : null}
            <h3 className="text-lg font-semibold leading-6 text-slate-50">
              {item.name}
            </h3>
            <p className="mt-2 line-clamp-2 text-sm leading-6 text-zinc-400">
              {item.description}
            </p>
          </div>
          <div className="shrink-0 text-left sm:text-right">
            <p className="text-sm text-zinc-500">Line total</p>
            <p className="mt-1 text-xl font-semibold text-neon-success">
              {formatMoney(item.price * item.quantity)}
            </p>
          </div>
        </div>

        <div className="mt-5 flex flex-col gap-3 border-t border-violet-400/15 pt-4 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex w-fit items-center rounded-2xl border border-violet-400/25 bg-black/25 p-1">
            <button
              type="button"
              onClick={onDecrease}
              className="h-10 w-10 rounded-xl text-lg font-semibold text-zinc-200 transition duration-200 hover:bg-violet-500/20 hover:text-white"
              aria-label={`Decrease ${item.name} quantity`}
            >
              -
            </button>
            <span className="flex h-10 min-w-12 items-center justify-center px-3 text-sm font-semibold text-white">
              {item.quantity}
            </span>
            <button
              type="button"
              onClick={onIncrease}
              className="h-10 w-10 rounded-xl text-lg font-semibold text-zinc-200 transition duration-200 hover:bg-violet-500/20 hover:text-white"
              aria-label={`Increase ${item.name} quantity`}
            >
              +
            </button>
          </div>

          <div className="flex items-center gap-3">
            <span className="rounded-full border border-violet-400/20 bg-black/25 px-3 py-1 text-sm text-zinc-300">
              {formatMoney(item.price)} each
            </span>
            <button
              type="button"
              onClick={onRemove}
              className="rounded-2xl border border-red-300/25 bg-red-400/10 px-4 py-2 text-sm font-semibold text-neon-error transition duration-200 hover:-translate-y-0.5 hover:border-red-300/45 hover:bg-red-400/15"
            >
              Remove
            </button>
          </div>
        </div>
      </div>
    </article>
  );
}

function SummaryRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between text-sm">
      <span className="text-zinc-400">{label}</span>
      <span className="font-semibold text-slate-100">{value}</span>
    </div>
  );
}

function CartEmptyState() {
  return (
    <GlassPanel className="min-h-64">
      <div className="flex h-56 items-center justify-center rounded-2xl border border-dashed border-violet-400/25 bg-black/20 text-center">
        <div>
          <p className="text-sm font-semibold uppercase tracking-[0.24em] text-neon-glow">
            Empty cart
          </p>
          <h3 className="mt-3 text-2xl font-semibold text-slate-100">
            No gear selected yet
          </h3>
          <p className="mx-auto mt-3 max-w-md text-sm leading-6 text-zinc-400">
            Add products from the catalog to stage a checkout payload.
          </p>
        </div>
      </div>
    </GlassPanel>
  );
}
