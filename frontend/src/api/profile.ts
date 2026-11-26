import { api } from './client'
import type { MessageResponse, Profile, Settings, UpdateSettingsRequest } from './types'

export async function getProfile(): Promise<Profile> {
  const { data } = await api.get<Profile>('/profile')
  return data
}

export async function getSettings(): Promise<Settings> {
  const { data } = await api.get<Settings>('/profile/settings')
  return data
}

/** Gemmer påmindelser og 2FA-metode. SMS kræver bekræftet telefonnummer (409 ellers). */
export async function updateSettings(request: UpdateSettingsRequest): Promise<Settings> {
  const { data } = await api.put<Settings>('/profile/settings', request)
  return data
}

/** Sender en SMS-kode til et nyt nummer. Nummeret gemmes først, når koden er bekræftet. */
export async function requestPhoneChange(phoneNumber: string): Promise<MessageResponse> {
  const { data } = await api.post<MessageResponse>('/profile/phone', { phoneNumber })
  return data
}

export async function confirmPhoneChange(phoneNumber: string, code: string): Promise<Settings> {
  const { data } = await api.post<Settings>('/profile/phone/confirm', { phoneNumber, code })
  return data
}
