import { api } from './client'
import type { InventoryItem } from './types'

export async function getInventory(): Promise<InventoryItem[]> {
  const { data } = await api.get<InventoryItem[]>('/inventory')
  return data
}

/** Udstyrer karakteren. Backend tager andre genstande i samme kategori af. */
export async function equipItem(inventoryId: number): Promise<InventoryItem> {
  const { data } = await api.post<InventoryItem>(`/inventory/${inventoryId}/equip`)
  return data
}

export async function unequipItem(inventoryId: number): Promise<InventoryItem> {
  const { data } = await api.post<InventoryItem>(`/inventory/${inventoryId}/unequip`)
  return data
}
