import { GlassPanel } from "../components/ui/GlassPanel";
import { PageHeader } from "../components/ui/PageHeader";

export function CartPage() {
  return (
    <div>
      <PageHeader
        eyebrow="Checkout"
        title="Cart staging bay"
        description="Cart state and checkout actions are intentionally not implemented in this first scaffold step."
      />

      <GlassPanel className="min-h-64">
        <div className="flex h-52 items-center justify-center rounded-2xl border border-dashed border-violet-400/25 bg-black/20 text-center">
          <div>
            <p className="text-lg font-semibold text-slate-100">
              Cart route is online
            </p>
            <p className="mt-2 text-sm text-zinc-400">
              Zustand cart state will be connected in a later step.
            </p>
          </div>
        </div>
      </GlassPanel>
    </div>
  );
}
