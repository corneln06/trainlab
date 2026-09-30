import {defineConfig} from "Vite";

export default defineConfig({
    optimizeDeps: {
        exclude: ["maplibre-gl"],
    },
});