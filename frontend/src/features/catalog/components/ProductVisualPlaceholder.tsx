type ProductVisualPlaceholderProps = {
  product: {
    name: string;
    category?: string;
  };
};

type PlaceholderKind =
  | "keyboard"
  | "mouse"
  | "monitor"
  | "processor"
  | "storage"
  | "accessories";

const placeholderMeta: Record<
  PlaceholderKind,
  {
    label: string;
    gradient: string;
    icon: string;
  }
> = {
  keyboard: {
    label: "Keyboard",
    gradient: "from-violet-500/40 via-fuchsia-500/20 to-cyan-400/20",
    icon: "⌘",
  },
  mouse: {
    label: "Mouse",
    gradient: "from-fuchsia-500/35 via-violet-500/20 to-emerald-400/20",
    icon: "◖",
  },
  monitor: {
    label: "Monitor",
    gradient: "from-cyan-400/30 via-violet-500/20 to-fuchsia-500/20",
    icon: "▣",
  },
  processor: {
    label: "Processor",
    gradient: "from-amber-300/25 via-violet-500/25 to-fuchsia-500/20",
    icon: "◈",
  },
  storage: {
    label: "Storage",
    gradient: "from-emerald-400/25 via-violet-500/25 to-cyan-400/20",
    icon: "▰",
  },
  accessories: {
    label: "Gear",
    gradient: "from-violet-500/35 via-purple-500/20 to-pink-400/20",
    icon: "✦",
  },
};

export function ProductVisualPlaceholder({
  product,
}: ProductVisualPlaceholderProps) {
  const kind = getPlaceholderKind(product);
  const meta = placeholderMeta[kind];

  return (
    <div
      className={[
        "relative flex aspect-[4/3] overflow-hidden rounded-2xl border border-violet-400/25",
        "bg-gradient-to-br",
        meta.gradient,
      ].join(" ")}
      aria-label={`${meta.label} visual placeholder`}
    >
      <div className="absolute inset-0 bg-[linear-gradient(135deg,rgba(255,255,255,0.08)_0_1px,transparent_1px)] bg-[length:18px_18px]" />
      <div className="absolute inset-x-7 bottom-7 top-7 rounded-2xl border border-white/10 bg-black/25 shadow-neon-soft" />
      <div className="absolute left-6 top-6 h-2 w-20 rounded-full bg-neon-glow/70 shadow-neon" />
      <div className="absolute bottom-6 right-6 h-14 w-14 rounded-2xl border border-white/15 bg-black/35 text-center text-4xl leading-[3.25rem] text-neon-glow shadow-neon">
        {meta.icon}
      </div>
      <div className="absolute bottom-6 left-6 rounded-full border border-violet-300/25 bg-black/35 px-3 py-1 text-xs font-semibold uppercase tracking-[0.18em] text-zinc-200">
        {meta.label}
      </div>
    </div>
  );
}

function getPlaceholderKind(product: ProductVisualPlaceholderProps["product"]): PlaceholderKind {
  const source = `${product.category ?? ""} ${product.name}`.toLowerCase();

  if (source.includes("keyboard")) return "keyboard";
  if (source.includes("mouse")) return "mouse";
  if (source.includes("monitor") || source.includes("display")) return "monitor";
  if (
    source.includes("processor") ||
    source.includes("cpu") ||
    source.includes("chip")
  ) {
    return "processor";
  }
  if (
    source.includes("storage") ||
    source.includes("ssd") ||
    source.includes("drive") ||
    source.includes("disk")
  ) {
    return "storage";
  }

  return "accessories";
}
