import { GlassPanel } from "../components/ui/GlassPanel";
import { PageHeader } from "../components/ui/PageHeader";

export function ProductsPage() {
  return (
    <div>
      <PageHeader
        eyebrow="Catalog"
        title="Products arena"
        description="The catalog route is ready for the live gateway contract. Product loading and recommendation flows will land in the next steps."
      />

      <GlassPanel>
        <div className="grid gap-4 md:grid-cols-3">
          {["Keyboard rigs", "Precision mice", "Battle stations"].map(
            (label) => (
              <div
                key={label}
                className="relative overflow-hidden rounded-2xl border border-violet-400/20 bg-black/25 p-5"
              >
                <div className="absolute inset-0 -translate-x-full bg-gradient-to-r from-transparent via-white/10 to-transparent animate-shimmer" />
                <div className="h-28 rounded-2xl border border-violet-400/20 bg-gradient-to-br from-violet-500/25 to-fuchsia-500/10" />
                <p className="mt-4 text-sm font-semibold text-slate-100">
                  {label}
                </p>
                <p className="mt-2 text-sm text-zinc-400">
                  Placeholder surface for the upcoming live catalog cards.
                </p>
              </div>
            ),
          )}
        </div>
      </GlassPanel>
    </div>
  );
}
