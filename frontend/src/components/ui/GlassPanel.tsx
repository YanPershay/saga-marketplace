import type { ReactNode } from "react";

type GlassPanelProps = {
  children: ReactNode;
  className?: string;
};

export function GlassPanel({ children, className = "" }: GlassPanelProps) {
  return (
    <section
      className={[
        "rounded-3xl border border-violet-400/25 bg-[rgba(20,20,32,0.75)]",
        "p-6 shadow-neon-soft backdrop-blur-xl transition duration-300",
        "hover:border-neon-glow/45 hover:shadow-neon",
        className,
      ].join(" ")}
    >
      {children}
    </section>
  );
}
