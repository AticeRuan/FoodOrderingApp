/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Components/**/*.{razor,html}",
        "./Pages/**/*.{razor,html}",
        "../FoodOrdering.Web.Client/**/*.{razor,html}"
    ],
    theme: {
        extend: {
            colors: {
                primary: {
                    DEFAULT: "#3b82f6",
                    foreground: "#ffffff",
                },
                secondary: {
                    DEFAULT: "#f3f4f6",
                    foreground: "#1f2937",
                },
                destructive: {
                    DEFAULT: "#ef4444",
                    foreground: "#ffffff",
                },
                muted: {
                    DEFAULT: "#f3f4f6",
                    foreground: "#6b7280",
                },
            },
        },
    },
    plugins: [],
}