import type { AuthUser } from '../api/types'

export interface StoredSession {
  accessToken: string
  expiresAt: string
  user: AuthUser
}

const STORAGE_KEY = 'levelup.session'

/** Henter den gemte session, hvis den findes og ikke er udløbet. */
export function loadSession(): StoredSession | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null

    const session = JSON.parse(raw) as StoredSession
    if (new Date(session.expiresAt).getTime() <= Date.now()) {
      localStorage.removeItem(STORAGE_KEY)
      return null
    }

    return session
  } catch {
    return null
  }
}

export function saveSession(session: StoredSession): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session))
  } catch {
    // Fx privat browsing – sessionen holdes så kun i hukommelsen
  }
}

export function clearSession(): void {
  try {
    localStorage.removeItem(STORAGE_KEY)
  } catch {
    // Ignorer
  }
}

/** Tokenet axios skal sende med. Holdes i hukommelsen, så det også virker uden localStorage. */
let currentToken: string | null = loadSession()?.accessToken ?? null

export function getAccessToken(): string | null {
  return currentToken
}

export function setAccessToken(token: string | null): void {
  currentToken = token
}
