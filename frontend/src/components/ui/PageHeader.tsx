type PageHeaderProps = {
  eyebrow: string;
  title: string;
  description: string;
};

export function PageHeader({ eyebrow, title, description }: PageHeaderProps) {
  return (
    <div className="mb-6 max-w-3xl">
      <p className="text-xs font-semibold uppercase tracking-[0.3em] text-neon-glow">
        {eyebrow}
      </p>
      <h2 className="mt-3 text-3xl font-semibold tracking-normal text-slate-50 sm:text-4xl">
        {title}
      </h2>
      <p className="mt-3 text-base leading-7 text-zinc-400">{description}</p>
    </div>
  );
}
