import { createContext } from 'react'
import type { AuthResponse, AuthUser, Profile } from '../api/types'

export interface AuthContextValue {
  /** Den loggede ind bruger, eller null. */
  user: AuthUser | null
  isAuthenticated: boolean
  /** Level, XP og points. Hentes fra API'et efter login. */
  profile: Profile | null
  /** Gemmer sessionen efter login trin 2. */
  signIn: (response: AuthResponse) => void
  signOut: () => void
  /** Henter profilen igen, fx. efter en opgave er gennemført. */
  refreshProfile: () => Promise<void>
}

export const AuthContext = createContext<AuthContextValue | null>(null)
