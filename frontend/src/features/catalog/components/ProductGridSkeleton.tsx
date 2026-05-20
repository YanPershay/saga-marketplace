export function ProductGridSkeleton() {
  return (
    <div className="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
      {Array.from({ length: 6 }).map((_, index) => (
        <div
          key={index}
          className="relative overflow-hidden rounded-3xl border border-violet-400/20 bg-[rgba(20,20,32,0.65)] p-4 shadow-neon-soft"
        >
          <div className="absolute inset-0 -translate-x-full animate-shimmer bg-gradient-to-r from-transparent via-white/10 to-transparent" />
          <div className="aspect-[4/3] rounded-2xl bg-violet-400/10" />
          <div className="mt-5 h-4 w-28 rounded-full bg-violet-400/15" />
          <div className="mt-4 h-5 w-4/5 rounded-full bg-zinc-300/10" />
          <div className="mt-3 h-4 w-full rounded-full bg-zinc-300/10" />
          <div className="mt-2 h-4 w-2/3 rounded-full bg-zinc-300/10" />
          <div className="mt-6 h-10 rounded-2xl bg-violet-400/10" />
        </div>
      ))}
    </div>
  );
}
