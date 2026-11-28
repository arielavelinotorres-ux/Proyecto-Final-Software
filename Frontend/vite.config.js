import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// Vite sirve la página mientras desarrollas (npm run dev) y la empaqueta (npm run build).
// VITE_PROXY apunta al backend Minimal API (puerto configurado en launchSettings.json).
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const backendUrl = env.VITE_PROXY || 'http://localhost:5296'
  return {
    base: mode === 'production' ? '/Proyecto-Final-Software/' : '/',
    plugins: [react()],
    server: { proxy: { '/api': { target: backendUrl, changeOrigin: true } } },
  }
})
