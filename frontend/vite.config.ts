import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Configuração do Vite para o projeto de controle de gastos
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
  },
})
