// Danske navne og ikoner for shoppens kategorier. Bruges både i shoppen og på karaktersiden.

export interface CategoryInfo {
  /** Flertal, til overskrifter i shoppen ("Hatte"). */
  label: string
  /** Ental, til pladserne på karakteren ("Hat"). */
  slot: string
  icon: string
}

/** Rækkefølgen her er også rækkefølgen pladserne vises i på karakteren. */
export const categories: Record<string, CategoryInfo> = {
  Hat: { label: 'Hatte', slot: 'Hat', icon: '🎩' },
  Outfit: { label: 'Tøj', slot: 'Tøj', icon: '👕' },
  Accessory: { label: 'Tilbehør', slot: 'Tilbehør', icon: '🗡️' },
}

/** Ukendte kategorier vises med deres eget navn og et gaveikon. */
export function categoryInfo(category: string): CategoryInfo {
  return categories[category] ?? { label: category, slot: category, icon: '🎁' }
}
