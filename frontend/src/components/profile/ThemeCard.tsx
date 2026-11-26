import { useState } from 'react'
import { getThemePreference, setThemePreference, type ThemePreference } from '../../utils/theme'
import { RadioGroup } from '../Form'

const options: { value: ThemePreference; label: string }[] = [
  { value: 'light', label: 'Lys' },
  { value: 'dark', label: 'Mørk' },
  { value: 'auto', label: 'Følg systemet' },
]

/** Lys/mørk tilstand. Skifter med det samme og gemmes i browseren, ikke på kontoen. */
export default function ThemeCard() {
  const [theme, setTheme] = useState(getThemePreference)

  function handleChange(value: ThemePreference) {
    setTheme(value)
    setThemePreference(value)
  }

  return (
    <div className="card">
      <div className="card-body pb-0">
        <h2 className="h6 card-title">Udseende</h2>
        <RadioGroup
          legend="Tema"
          name="theme"
          value={theme}
          options={options}
          onChange={handleChange}
          hint="Gemmes på denne enhed."
        />
      </div>
    </div>
  )
}
