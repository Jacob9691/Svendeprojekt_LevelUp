// Lys/mørk tilstand. Valget gemmes i browseren (pr. enhed), så det også gælder på login-siden.
// Bootstrap skifter farver ud fra data-bs-theme på <html>, FullCalendar ud fra data-color-scheme.
// index.html sætter temaet, før appen starter, så siden ikke blinker hvid ved indlæsning.

export type ThemePreference = 'light' | 'dark' | 'auto'

const STORAGE_KEY = 'levelup.theme'
const systemDark = window.matchMedia('(prefers-color-scheme: dark)')

export function getThemePreference(): ThemePreference {
  try {
    const value = localStorage.getItem(STORAGE_KEY)
    return value === 'light' || value === 'dark' ? value : 'auto'
  } catch {
    return 'auto'
  }
}

export function setThemePreference(preference: ThemePreference): void {
  try {
    if (preference === 'auto') localStorage.removeItem(STORAGE_KEY)
    else localStorage.setItem(STORAGE_KEY, preference)
  } catch {
    // Fx privat browsing, valget gælder så kun indtil siden genindlæses
  }
  applyTheme(preference)
}

function applyTheme(preference: ThemePreference): void {
  const theme = preference === 'auto' ? (systemDark.matches ? 'dark' : 'light') : preference
  document.documentElement.dataset.bsTheme = theme
  document.documentElement.dataset.colorScheme = theme
}

/** Kaldes én gang ved opstart. Følger systemets tema, hvis brugeren har valgt "Følg systemet". */
export function initTheme(): void {
  applyTheme(getThemePreference())
  systemDark.addEventListener('change', () => {
    if (getThemePreference() === 'auto') applyTheme('auto')
  })
}
