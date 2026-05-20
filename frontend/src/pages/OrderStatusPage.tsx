import { useParams } from "react-router-dom";
import { GlassPanel } from "../components/ui/GlassPanel";
import { PageHeader } from "../components/ui/PageHeader";

const demoSteps = ["Pending", "AwaitingInventory", "Completed"];

export function OrderStatusPage() {
  const { orderId } = useParams();

  return (
    <div>
      <PageHeader
        eyebrow="Order saga"
        title="Timeline bridge"
        description="This route is wired for future order polling. The visual timeline is static for now."
      />

      <GlassPanel>
        <div className="mb-5 rounded-2xl border border-violet-400/20 bg-black/25 px-4 py-3">
          <p className="text-xs uppercase tracking-[0.24em] text-zinc-500">
            Route parameter
          </p>
          <p className="mt-1 break-all font-mono text-sm text-neon-glow">
            {orderId}
          </p>
        </div>

        <div className="space-y-4">
          {demoSteps.map((step, index) => (
            <div key={step} className="flex gap-4">
              <div className="flex flex-col items-center">
                <div className="h-4 w-4 rounded-full border border-neon-glow bg-violet-500 shadow-neon transition duration-300" />
                {index < demoSteps.length - 1 ? (
                  <div className="h-12 w-px bg-violet-400/30" />
                ) : null}
              </div>
              <div>
                <p className="font-semibold text-slate-100">{step}</p>
                <p className="text-sm text-zinc-400">
                  Static scaffold state, ready for live saga polling.
                </p>
              </div>
            </div>
          ))}
        </div>
      </GlassPanel>
    </div>
  );
}
