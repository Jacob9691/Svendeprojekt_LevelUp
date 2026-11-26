import { api } from './client'
import type { Achievement, Challenge } from './types'

export async function getAchievements(): Promise<Achievement[]> {
  const { data } = await api.get<Achievement[]>('/achievements')
  return data
}

export async function getChallenges(): Promise<Challenge[]> {
  const { data } = await api.get<Challenge[]>('/challenges')
  return data
}

/** Starter udfordringen. Tidspunkterne er for den første opgave – resten følger gentagelsen. */
export async function startChallenge(id: number, scheduledStart: string, scheduledEnd: string | null): Promise<Challenge> {
  const { data } = await api.post<Challenge>(`/challenges/${id}/start`, { scheduledStart, scheduledEnd })
  return data
}

/** Opgiver udfordringen. Ikke-gennemførte opgaver fjernes fra kalenderen. */
export async function abandonChallenge(id: number): Promise<void> {
  await api.delete(`/challenges/${id}`)
}
