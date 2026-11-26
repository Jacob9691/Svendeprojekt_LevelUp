import axios from 'axios'
import { getAccessToken } from '../auth/tokenStorage'

export const api = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL}/api`,
  headers: { 'Content-Type': 'application/json' },
})

// Sender access token med på alle requests, når brugeren er logget ind
api.interceptors.request.use((config) => {
  const token = getAccessToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

let onUnauthorized: (() => void) | null = null

/** AuthProvider registrerer her, hvad der skal ske når tokenet ikke længere er gyldigt. */
export function setUnauthorizedHandler(handler: (() => void) | null): void {
  onUnauthorized = handler
}

// Et 401-svar på en request med token betyder at tokenet er udløbet - log ud.
// (401 ved login uden token er bare forkert adgangskode og håndteres af siden.)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (axios.isAxiosError(error) && error.response?.status === 401 && error.config?.headers?.Authorization) {
      onUnauthorized?.()
    }
    return Promise.reject(error)
  },
)
