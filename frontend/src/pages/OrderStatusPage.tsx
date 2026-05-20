import { useParams } from "react-router-dom";
import { GlassPanel } from "../components/ui/GlassPanel";
import { PageHeader } from "../components/ui/PageHeader";
import { useOrderQuery } from "../features/order-status/hooks/useOrderQuery";
import { formatMoney } from "../lib/money";
import {
  getOrderStatusTone,
  getTimelineNotice,
  getTimelineStepState,
  isTerminalOrderStatus,
  orderTimelineStages,
  type TimelineStepState,
} from "../lib/orderStatus";
import type { Order, OrderStatus } from "../types/order";

export function OrderStatusPage() {
  const { orderId } = useParams();
  const {
    data: order,
    isLoading,
    isError,
    error,
    refetch,
    isFetching,
  } = useOrderQuery(orderId);
  const isTerminal = isTerminalOrderStatus(order?.status);

  return (
    <div>
      <PageHeader
        eyebrow="Order saga"
        title="Live saga timeline"
        description="Track the distributed order flow as inventory, payment, and shipping services advance the saga."
      />

      {isLoading ? <OrderStatusSkeleton /> : null}

      {isError ? (
        <OrderErrorState
          message={error instanceof Error ? error.message : "Unable to load order."}
          orderId={orderId}
          onRetry={() => void refetch()}
        />
      ) : null}

      {order ? (
        <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_24rem]">
          <div className="space-y-6">
            <OrderOverview
              order={order}
              isFetching={isFetching}
              isTerminal={isTerminal}
            />
            <OrderItemsPanel order={order} />
          </div>

          <SagaTimeline order={order} />
        </div>
      ) : null}
    </div>
  );
}

function OrderOverview({
  order,
  isFetching,
  isTerminal,
}: {
  order: Order;
  isFetching: boolean;
  isTerminal: boolean;
}) {
  return (
    <GlassPanel>
      <div className="flex flex-col gap-5 md:flex-row md:items-start md:justify-between">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.24em] text-neon-glow">
            Order status
          </p>
          <h3 className="mt-3 break-all font-mono text-lg font-semibold text-slate-50">
            {order.id}
          </h3>
        </div>
        <div className="flex flex-wrap gap-2">
          <StatusBadge status={order.status} />
          {!isTerminal ? (
            <span className="rounded-full border border-violet-300/25 bg-violet-500/10 px-3 py-1 text-sm font-semibold text-neon-glow transition duration-300">
              {isFetching ? "Refreshing..." : "Polling every 2s"}
            </span>
          ) : (
            <span className="rounded-full border border-zinc-400/20 bg-zinc-400/10 px-3 py-1 text-sm font-semibold text-zinc-300">
              Polling stopped
            </span>
          )}
        </div>
      </div>

      <div className="mt-6 grid gap-4 sm:grid-cols-3">
        <InfoTile label="Customer" value={order.customerId} mono />
        <InfoTile label="Created" value={formatDate(order.createdAt)} />
        <InfoTile label="Total" value={formatMoney(order.totalPrice)} strong />
      </div>
    </GlassPanel>
  );
}

function OrderItemsPanel({ order }: { order: Order }) {
  return (
    <GlassPanel>
      <div className="flex items-center justify-between gap-4">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.24em] text-neon-glow">
            Items
          </p>
          <h3 className="mt-2 text-2xl font-semibold text-slate-50">
            Order payload
          </h3>
        </div>
        <span className="rounded-full border border-violet-400/20 bg-black/25 px-3 py-1 text-sm text-zinc-300">
          {order.items.length} lines
        </span>
      </div>

      <div className="mt-5 space-y-3">
        {order.items.map((item) => (
          <div
            key={item.productId}
            className="grid gap-3 rounded-2xl border border-violet-400/20 bg-black/25 p-4 sm:grid-cols-[minmax(0,1fr)_7rem_8rem]"
          >
            <div className="min-w-0">
              <p className="text-xs uppercase tracking-[0.2em] text-zinc-500">
                Product
              </p>
              <p className="mt-1 break-all font-mono text-sm text-zinc-200">
                {item.productId}
              </p>
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.2em] text-zinc-500">
                Qty
              </p>
              <p className="mt-1 text-sm font-semibold text-slate-100">
                {item.quantity}
              </p>
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.2em] text-zinc-500">
                Price
              </p>
              <p className="mt-1 text-sm font-semibold text-neon-success">
                {formatMoney(item.price)}
              </p>
            </div>
          </div>
        ))}
      </div>
    </GlassPanel>
  );
}

function SagaTimeline({ order }: { order: Order }) {
  const notice = getTimelineNotice(order.status);

  return (
    <GlassPanel className="h-fit">
      <p className="text-xs font-semibold uppercase tracking-[0.24em] text-neon-glow">
        Saga timeline
      </p>
      <h3 className="mt-2 text-2xl font-semibold text-slate-50">
        {order.status === "Failed"
          ? "Saga failed"
          : order.status === "Cancelled"
            ? "Saga cancelled"
            : notice
              ? "Safe fallback"
              : "Progression"}
      </h3>

      <div className="mt-6 space-y-4">
        {notice ? (
          <TimelineNotice
            label={notice.label}
            description={notice.description}
            tone={notice.tone}
          />
        ) : null}

        {orderTimelineStages.map((stage, index) => {
          const stepState = getTimelineStepState(order.status, stage.status);

          return (
            <div key={stage.status} className="flex gap-4">
              <div className="flex flex-col items-center">
                <div
                  className={[
                    "h-4 w-4 rounded-full border transition duration-300",
                    getTimelineDotClass(stepState, order.status),
                  ].join(" ")}
                />
                {index < orderTimelineStages.length - 1 ? (
                  <div
                    className={[
                      "h-14 w-px transition duration-300",
                      stepState === "completed"
                        ? "bg-violet-400/60"
                        : "bg-violet-400/20",
                    ].join(" ")}
                  />
                ) : null}
              </div>
              <div className="pb-2">
                <p
                  className={[
                    "font-semibold transition duration-300",
                    stepState === "completed" || stepState === "current"
                      ? "text-slate-50"
                      : "text-zinc-500",
                  ].join(" ")}
                >
                  {stage.label}
                </p>
                <p className="mt-1 text-sm leading-6 text-zinc-400">
                  {stage.description}
                </p>
              </div>
            </div>
          );
        })}
      </div>
    </GlassPanel>
  );
}

function TimelineNotice({
  label,
  description,
  tone,
}: {
  label: string;
  description: string;
  tone: "active" | "danger";
}) {
  return (
    <div
      className={[
        "flex gap-4 rounded-2xl border p-4",
        tone === "danger"
          ? "border-red-300/20 bg-red-400/10"
          : "border-violet-300/20 bg-violet-500/10",
      ].join(" ")}
    >
      <div className="pt-1">
        <div
          className={[
            "h-4 w-4 rounded-full border transition duration-300",
            tone === "danger"
              ? "border-red-300 bg-red-400 shadow-[0_0_26px_rgba(248,113,113,0.35)]"
              : "border-neon-glow bg-violet-500 shadow-neon",
          ].join(" ")}
        />
      </div>
      <div>
        <p className="font-semibold text-slate-50">{label}</p>
        <p className="mt-1 text-sm leading-6 text-zinc-400">{description}</p>
      </div>
    </div>
  );
}

function StatusBadge({ status }: { status: OrderStatus }) {
  const tone = getOrderStatusTone(status);
  const className =
    tone === "success"
      ? "border-emerald-300/25 bg-emerald-400/10 text-neon-success"
      : tone === "danger"
        ? "border-red-300/25 bg-red-400/10 text-neon-error"
        : "border-violet-300/25 bg-violet-500/10 text-neon-glow";

  return (
    <span
      className={[
        "rounded-full border px-3 py-1 text-sm font-semibold transition duration-300",
        className,
      ].join(" ")}
    >
      {status}
    </span>
  );
}

function InfoTile({
  label,
  value,
  mono = false,
  strong = false,
}: {
  label: string;
  value: string;
  mono?: boolean;
  strong?: boolean;
}) {
  return (
    <div className="rounded-2xl border border-violet-400/20 bg-black/25 p-4">
      <p className="text-xs uppercase tracking-[0.22em] text-zinc-500">{label}</p>
      <p
        className={[
          "mt-2 break-all text-sm",
          mono ? "font-mono" : "font-semibold",
          strong ? "text-neon-success" : "text-zinc-200",
        ].join(" ")}
      >
        {value}
      </p>
    </div>
  );
}

function OrderStatusSkeleton() {
  return (
    <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_24rem]">
      <GlassPanel>
        <div className="relative overflow-hidden rounded-2xl border border-violet-400/15 bg-black/20 p-5">
          <div className="absolute inset-0 -translate-x-full animate-shimmer bg-gradient-to-r from-transparent via-white/10 to-transparent" />
          <div className="h-4 w-36 rounded-full bg-violet-400/15" />
          <div className="mt-4 h-6 w-4/5 rounded-full bg-zinc-300/10" />
          <div className="mt-6 grid gap-4 sm:grid-cols-3">
            <div className="h-24 rounded-2xl bg-violet-400/10" />
            <div className="h-24 rounded-2xl bg-violet-400/10" />
            <div className="h-24 rounded-2xl bg-violet-400/10" />
          </div>
        </div>
      </GlassPanel>
      <GlassPanel>
        <div className="space-y-4">
          {Array.from({ length: 6 }).map((_, index) => (
            <div key={index} className="h-16 rounded-2xl bg-violet-400/10" />
          ))}
        </div>
      </GlassPanel>
    </div>
  );
}

function OrderErrorState({
  message,
  orderId,
  onRetry,
}: {
  message: string;
  orderId?: string;
  onRetry: () => void;
}) {
  return (
    <div className="rounded-3xl border border-red-300/25 bg-red-400/10 p-6 shadow-neon-soft backdrop-blur-xl">
      <p className="text-sm font-semibold uppercase tracking-[0.24em] text-neon-error">
        Order unavailable
      </p>
      <h3 className="mt-3 text-2xl font-semibold text-slate-50">
        Could not load order status
      </h3>
      {orderId ? (
        <p className="mt-3 break-all font-mono text-sm text-zinc-300">
          {orderId}
        </p>
      ) : null}
      <p className="mt-3 max-w-3xl text-sm leading-6 text-zinc-300">{message}</p>
      <button
        type="button"
        onClick={onRetry}
        className="mt-5 rounded-2xl border border-violet-300/35 bg-violet-500/15 px-4 py-2 text-sm font-semibold text-white transition duration-200 hover:-translate-y-0.5 hover:border-neon-glow/70 hover:bg-violet-500/25 hover:shadow-neon"
      >
        Retry order
      </button>
    </div>
  );
}

function getTimelineDotClass(stepState: TimelineStepState, status: OrderStatus) {
  if (stepState === "current" && status === "Completed") {
    return "border-emerald-300 bg-emerald-400 shadow-[0_0_26px_rgba(52,211,153,0.35)]";
  }

  if (stepState === "completed" || stepState === "current") {
    return "border-neon-glow bg-violet-500 shadow-neon";
  }

  return "border-violet-400/25 bg-black/40";
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("en-US", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}
