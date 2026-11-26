// Danske navne og ikoner for shoppens kategorier. Bruges både i shoppen og på karaktersiden.

import type { IconName } from './icons'

export interface CategoryInfo {
  /** Flertal, til overskrifter i shoppen ("Hatte"). */
  label: string
  /** Ental, til pladserne på karakteren ("Hat"). */
  slot: string
  icon: IconName
}

/** Rækkefølgen her er også rækkefølgen pladserne vises i på karakteren. */
export const categories: Record<string, CategoryInfo> = {
  Hat: { label: 'Hatte', slot: 'Hat', icon: 'hat' },
  Outfit: { label: 'Tøj', slot: 'Tøj', icon: 'outfit' },
  Accessory: { label: 'Tilbehør', slot: 'Tilbehør', icon: 'accessory' },
}

/** Ukendte kategorier vises med deres eget navn og et gaveikon. */
export function categoryInfo(category: string): CategoryInfo {
  return categories[category] ?? { label: category, slot: category, icon: 'unknownItem' }
}
