import { api } from './client'
import type { CompleteTaskResponse, CreateTaskRequest, SaveTaskRequest, TaskItem } from './types'

/** Opgaver der starter i perioden (from, to). Datoerne er lokale tidspunkter, se utils/dates.ts. */
export async function getTasks(from: string, to: string): Promise<TaskItem[]> {
  const { data } = await api.get<TaskItem[]>('/tasks', { params: { from, to } })
  return data
}

/** Opretter opgaven, eller en hel serie, hvis repeat er sat. Returnerer den første. */
export async function createTask(request: CreateTaskRequest): Promise<TaskItem> {
  const { data } = await api.post<TaskItem>('/tasks', request)
  return data
}

export async function updateTask(id: number, request: SaveTaskRequest): Promise<TaskItem> {
  const { data } = await api.put<TaskItem>(`/tasks/${id}`, request)
  return data
}

/** Med following=true slettes også senere, ikke-gennemførte opgaver i samme serie. */
export async function deleteTask(id: number, following = false): Promise<void> {
  await api.delete(`/tasks/${id}`, { params: following ? { following } : undefined })
}

/** Markerer opgaven som gennemført. Backend giver XP og points. */
export async function completeTask(id: number): Promise<CompleteTaskResponse> {
  const { data } = await api.post<CompleteTaskResponse>(`/tasks/${id}/complete`)
  return data
}
