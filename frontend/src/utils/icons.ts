// Alle ikoner i appen, samlet ét sted. Navnet til venstre er det koden bruger (<GameIcon name="hat" />),
// "file" er SVG-filen i src/assets/icons (uden .svg). Se README.md i den mappe.
//
// Findes filen ikke, vises emojien i "fallback" i stedet. Så kan ikonerne tilføjes én ad gangen.

export interface IconDefinition {
  /** Filnavnet i src/assets/icons uden ".svg", fx "pointy-hat". */
  file: string
  /** Vises indtil SVG-filen er lagt i mappen. */
  fallback: string
  /**
   * true: behold farverne fra filen (hvis ikonet er farvelagt på game-icons.net).
   * false (standard): ikonet får tekstfarven, så det følger Bootstrap-klasser som text-success.
   */
  colors?: boolean
}

export const iconRegistry = {
  // Shoppens kategorier
  hat: { file: 'pointy-hat', fallback: '🎩', colors: true },
  outfit: { file: 'armor-vest', fallback: '👕', colors: true },
  accessory: { file: 'sword', fallback: '🗡️', colors: true },
  unknownItem: { file: 'present', fallback: '🎁', colors: true },

  // Karakteren
  character: { file: 'character', fallback: '🧙', colors: true },

  // Achievements
  level: { file: 'star', fallback: '⭐', colors: true },
  tasksCompleted: { file: 'check-mark', fallback: '✅', colors: true },
  itemsBought: { file: 'shopping-bag', fallback: '🛍️', colors: true },
  challengesCompleted: { file: 'stairs-goal', fallback: '🏁', colors: true },
  trophy: { file: 'achievement', fallback: '🏆', colors: true },
} satisfies Record<string, IconDefinition>

export type IconName = keyof typeof iconRegistry
