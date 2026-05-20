import { NavLink, Outlet } from "react-router-dom";
import { API_BASE_URL } from "../../api/config";

const navItems = [
  { to: "/products", label: "Catalog" },
  { to: "/cart", label: "Cart" },
  { to: "/orders/demo-order-id", label: "Saga Timeline" },
];

export function AppLayout() {
  return (
    <div className="min-h-screen overflow-hidden bg-abyss-950 text-slate-50">
      <div className="pointer-events-none fixed inset-0 bg-radial-grid bg-[length:100%_100%,100%_100%,28px_28px]" />
      <div className="pointer-events-none fixed inset-x-0 top-0 h-48 bg-gradient-to-b from-neon-primary/20 to-transparent blur-3xl" />

      <div className="relative mx-auto flex min-h-screen w-full max-w-7xl flex-col px-5 py-5 sm:px-8 lg:px-10">
        <header className="sticky top-5 z-20 rounded-3xl border border-violet-400/25 bg-[rgba(20,20,32,0.75)] px-5 py-4 shadow-neon-soft backdrop-blur-xl">
          <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.28em] text-neon-glow">
                Saga Marketplace
              </p>
              <h1 className="mt-1 text-2xl font-semibold tracking-normal text-slate-50">
                Neon demo control room
              </h1>
            </div>

            <nav className="flex flex-wrap gap-2">
              {navItems.map((item) => (
                <NavLink
                  key={item.to}
                  to={item.to}
                  className={({ isActive }) =>
                    [
                      "rounded-2xl border px-4 py-2 text-sm font-medium transition duration-200",
                      "hover:-translate-y-0.5 hover:border-neon-glow/60 hover:bg-violet-500/15 hover:shadow-neon",
                      isActive
                        ? "border-neon-primary/70 bg-violet-500/20 text-white shadow-neon"
                        : "border-violet-400/20 bg-white/[0.03] text-zinc-300",
                    ].join(" ")
                  }
                >
                  {item.label}
                </NavLink>
              ))}
            </nav>
          </div>
        </header>

        <main className="flex-1 py-8">
          <Outlet />
        </main>

        <footer className="pb-4 text-xs text-zinc-400">
          <div className="rounded-2xl border border-violet-400/20 bg-black/20 px-4 py-3 backdrop-blur">
            Gateway base:{" "}
            <span className="font-mono text-zinc-200">
              {API_BASE_URL || "VITE_API_BASE_URL is not set"}
            </span>
          </div>
        </footer>
      </div>
    </div>
  );
}
