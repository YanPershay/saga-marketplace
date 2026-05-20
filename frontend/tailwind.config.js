/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        abyss: {
          950: "#050509",
          900: "#0B0B12",
          850: "#11111A",
        },
        neon: {
          primary: "#8B5CF6",
          secondary: "#A855F7",
          glow: "#C084FC",
          success: "#34D399",
          warning: "#FBBF24",
          error: "#F87171",
        },
      },
      boxShadow: {
        neon: "0 0 26px rgba(139, 92, 246, 0.28)",
        "neon-soft": "0 0 42px rgba(192, 132, 252, 0.16)",
      },
      backgroundImage: {
        "radial-grid":
          "radial-gradient(circle at top left, rgba(139, 92, 246, 0.22), transparent 34%), radial-gradient(circle at top right, rgba(168, 85, 247, 0.12), transparent 30%), linear-gradient(135deg, rgba(139, 92, 246, 0.08) 0 1px, transparent 1px)",
      },
      keyframes: {
        shimmer: {
          "0%": { transform: "translateX(-100%)" },
          "100%": { transform: "translateX(100%)" },
        },
        pulseGlow: {
          "0%, 100%": { opacity: "0.52" },
          "50%": { opacity: "0.88" },
        },
      },
      animation: {
        shimmer: "shimmer 1.8s ease-in-out infinite",
        "pulse-glow": "pulseGlow 3s ease-in-out infinite",
      },
    },
  },
  plugins: [],
};
