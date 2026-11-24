import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Backend'ens CORS-policy tillader http://localhost:5173
    port: 5173,
    strictPort: true,
  },
})
