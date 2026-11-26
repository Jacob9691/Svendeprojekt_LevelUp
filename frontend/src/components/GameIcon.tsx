import { iconRegistry, type IconDefinition, type IconName } from '../utils/icons'

// Alle .svg-filer i src/assets/icons bundtes med appen som tekst. Nøglen bliver filnavnet uden ".svg".
const files = import.meta.glob<string>('../assets/icons/*.svg', { query: '?raw', import: 'default', eager: true })
const svgByFile = new Map(
  Object.entries(files).map(([path, svg]) => [path.slice(path.lastIndexOf('/') + 1, -'.svg'.length), svg]),
)

/** Den sorte firkant game-icons.net lægger bag ikonet, når man downloader med standardindstillingerne. */
const BACKGROUND_PATH = 'M0 0h512v512H0z'

/**
 * Gør SVG-filen klar til at blive vist inde i teksten: fylder 1em × 1em, og (medmindre ikonet har
 * egne farver) fjernes baggrunden og alt farves med tekstfarven (currentColor).
 */
function prepareSvg(svg: string, keepColors: boolean): string | null {
  const root = new DOMParser().parseFromString(svg, 'image/svg+xml').documentElement
  if (root.nodeName !== 'svg') return null // Ugyldig fil

  root.querySelectorAll('script, foreignObject').forEach((el) => el.remove())
  root.removeAttribute('style')
  root.setAttribute('width', '100%')
  root.setAttribute('height', '100%')
  if (!root.hasAttribute('viewBox')) root.setAttribute('viewBox', '0 0 512 512')

  if (!keepColors) {
    root.querySelectorAll(`path[d="${BACKGROUND_PATH}"]`).forEach((el) => el.remove())
    root.querySelectorAll('[fill]').forEach((el) => {
      if (el.getAttribute('fill') !== 'none') el.setAttribute('fill', 'currentColor')
    })
    root.setAttribute('fill', 'currentColor')
  }

  return new XMLSerializer().serializeToString(root)
}

// Klargøres én gang, når appen starter
const prepared = new Map<IconName, string | null>(
  (Object.entries(iconRegistry) as [IconName, IconDefinition][]).map(([name, icon]) => {
    const svg = svgByFile.get(icon.file)
    return [name, svg ? prepareSvg(svg, icon.colors ?? false) : null]
  }),
)

if (import.meta.env.DEV) {
  const missing = Object.values(iconRegistry as Record<string, IconDefinition>)
    .filter((icon) => !svgByFile.has(icon.file))
    .map((icon) => `${icon.file}.svg`)
  if (missing.length > 0) {
    console.info(`[GameIcon] Disse ikoner mangler i src/assets/icons og vises som emoji: ${missing.join(', ')}`)
  }
}

interface GameIconProps {
  name: IconName
  className?: string
  /** Tekst til skærmlæsere. Uden title er ikonet kun pynt og skjules for dem. */
  title?: string
}

/**
 * Ikon fra game-icons.net (src/assets/icons). Størrelsen følger skriftstørrelsen, så brug fx
 * className="fs-1", og farven følger tekstfarven, fx className="text-success".
 */
export default function GameIcon({ name, className, title }: GameIconProps) {
  const svg = prepared.get(name)
  const a11y = title ? { role: 'img', 'aria-label': title, title } : { 'aria-hidden': true }
  const classes = `d-inline-block${className ? ` ${className}` : ''}`

  if (!svg) {
    return (
      <span className={classes} {...a11y}>
        {iconRegistry[name].fallback}
      </span>
    )
  }

  return (
    <span
      className={classes}
      style={{ width: '1em', height: '1em', lineHeight: 0, verticalAlign: '-0.125em' }}
      {...a11y}
      // Filerne ligger i projektet og bundtes ved build, de kommer ikke fra brugere
      dangerouslySetInnerHTML={{ __html: svg }}
    />
  )
}
