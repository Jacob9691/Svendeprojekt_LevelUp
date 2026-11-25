import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'
import { setUnauthorizedHandler } from '../api/client'
import { getProfile } from '../api/profile'
import type { AuthResponse, Profile } from '../api/types'
import { AuthContext, type AuthContextValue } from './AuthContext'
import {
  clearSession,
  loadSession,
  saveSession,
  setAccessToken,
  type StoredSession,
} from './tokenStorage'

/**
 * Holder styr på om brugeren er logget ind. Sessionen (JWT + bruger) gemmes i localStorage,
 * så man forbliver logget ind ved genindlæsning, indtil tokenet udløber.
 */
export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<StoredSession | null>(loadSession)
  const [profile, setProfile] = useState<Profile | null>(null)

  const signOut = useCallback(() => {
    clearSession()
    setAccessToken(null)
    setSession(null)
    setProfile(null)
  }, [])

  const signIn = useCallback((response: AuthResponse) => {
    const newSession: StoredSession = {
      accessToken: response.accessToken,
      expiresAt: response.expiresAt,
      user: response.user,
    }
    saveSession(newSession)
    setAccessToken(newSession.accessToken)
    setSession(newSession)
  }, [])

  const refreshProfile = useCallback(async () => {
    setProfile(await getProfile())
  }, [])

  // Log ud hvis API'et svarer 401 på et kald med token (tokenet er udløbet eller ugyldigt)
  useEffect(() => {
    setUnauthorizedHandler(signOut)
    return () => setUnauthorizedHandler(null)
  }, [signOut])

  // Log automatisk ud når tokenet udløber
  useEffect(() => {
    if (!session) return

    const msLeft = new Date(session.expiresAt).getTime() - Date.now()
    const timer = window.setTimeout(signOut, Math.max(0, msLeft))
    return () => window.clearTimeout(timer)
  }, [session, signOut])

  // Hent profilen (level, XP, points) når brugeren er logget ind
  useEffect(() => {
    if (!session) return

    getProfile()
      .then(setProfile)
      .catch(() => {
        // 401 håndteres af interceptoren – andre fejl betyder bare at profilen ikke vises endnu
      })
  }, [session])

  const value = useMemo<AuthContextValue>(
    () => ({
      user: session?.user ?? null,
      isAuthenticated: session !== null,
      profile,
      signIn,
      signOut,
      refreshProfile,
    }),
    [session, profile, signIn, signOut, refreshProfile],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
