import { api } from './client'
import type {
  AuthResponse,
  ConfirmAccountRequest,
  LoginRequest,
  LoginResponse,
  MessageResponse,
  RegisterRequest,
  RegisterResponse,
  TwoFactorMethod,
} from './types'

export async function register(request: RegisterRequest): Promise<RegisterResponse> {
  const { data } = await api.post<RegisterResponse>('/auth/register', request)
  return data
}

export async function confirmAccount(request: ConfirmAccountRequest): Promise<MessageResponse> {
  const { data } = await api.post<MessageResponse>('/auth/confirm', request)
  return data
}

export async function resendConfirmation(email: string, method: TwoFactorMethod): Promise<MessageResponse> {
  const { data } = await api.post<MessageResponse>('/auth/confirm/resend', { email, method })
  return data
}

/** Login trin 1: e-mail og adgangskode. Sender en engangskode. */
export async function login(request: LoginRequest): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>('/auth/login', request)
  return data
}

/** Login trin 2: engangskoden. Returnerer access token. */
export async function verifyTwoFactor(twoFactorToken: string, code: string): Promise<AuthResponse> {
  const { data } = await api.post<AuthResponse>('/auth/login/2fa', { twoFactorToken, code })
  return data
}

export async function resendTwoFactor(twoFactorToken: string): Promise<LoginResponse> {
  const { data } = await api.post<LoginResponse>('/auth/login/2fa/resend', { twoFactorToken })
  return data
}
