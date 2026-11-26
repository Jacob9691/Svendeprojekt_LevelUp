import { api } from './client'
import type { PurchaseResponse, ShopItem } from './types'

export async function getShopItems(): Promise<ShopItem[]> {
  const { data } = await api.get<ShopItem[]>('/shop/items')
  return data
}

/** Køber en genstand. Backend tjekker points og om den allerede er købt (409 ellers). */
export async function purchaseItem(id: number): Promise<PurchaseResponse> {
  const { data } = await api.post<PurchaseResponse>(`/shop/items/${id}/purchase`)
  return data
}
