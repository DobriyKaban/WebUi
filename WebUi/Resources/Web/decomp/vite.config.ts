import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { viteSingleFile } from "vite-plugin-singlefile";

export default defineConfig({
    plugins: [react(), viteSingleFile()],
    build: {
        // Опционально: минификация, чтобы файл был меньше
        minify: true, 
    },
});