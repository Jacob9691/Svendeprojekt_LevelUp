// Typer der matcher backendens DTO'er (backend/LevelUp.Api/Dtos)

export type TwoFactorMethod = 'Email' | 'Sms'

export interface AuthUser {
  id: number
  email: string
  displayName: string
}

export interface RegisterRequest {
  email: string
  password: string
  displayName: string
  phoneNumber?: string
  confirmationMethod: TwoFactorMethod
}

export interface RegisterResponse {
  message: string
  confirmationMethod: TwoFactorMethod
  sentTo: string
}

export interface ConfirmAccountRequest {
  email: string
  code: string
  method: TwoFactorMethod
}

export interface LoginRequest {
  email: string
  password: string
}

/** Svar på login trin 1 – koden er sendt. */
export interface LoginResponse {
  twoFactorToken: string
  method: TwoFactorMethod
  sentTo: string
}

/** Svar på login trin 2 – brugeren er logget ind. */
export interface AuthResponse {
  accessToken: string
  expiresAt: string
  user: AuthUser
}

export interface MessageResponse {
  message: string
}

export interface Profile {
  id: number
  email: string
  displayName: string
  level: number
  totalXp: number
  xpIntoLevel: number
  xpForNextLevel: number
  coinBalance: number
  completedTasks: number
  inventoryCount: number
  createdAt: string
}

/** Fejlsvar fra API'et (ProblemDetails). */
export interface ProblemDetails {
  title?: string
  status?: number
  detail?: string
  code?: string
  errors?: Record<string, string[]>
}
