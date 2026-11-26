// Opgavernes tidspunkter gemmes som lokal tid uden tidszone ("2026-11-25T09:00:00"),
// så en opgave kl. 9 altid står kl. 9 i kalenderen. Derfor bruges toISOString() ikke (den giver UTC).

const pad = (n: number) => String(n).padStart(2, '0')

/** Date -> "2026-11-25T09:00:00" (til API'et). */
export function toLocalIso(date: Date): string {
  return `${toDateInputValue(date)}T${pad(date.getHours())}:${pad(date.getMinutes())}:00`
}

/** Date -> "2026-11-25" (til <input type="date">). */
export function toDateInputValue(date: Date): string {
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`
}

/** Date -> "09:00" (til <input type="time">). */
export function toTimeInputValue(date: Date): string {
  return `${pad(date.getHours())}:${pad(date.getMinutes())}`
}

/** "2026-11-25" + "09:00" -> Date i lokal tid. */
export function fromDateAndTime(date: string, time: string): Date {
  const [year, month, day] = date.split('-').map(Number)
  const [hours, minutes] = time.split(':').map(Number)
  return new Date(year, month - 1, day, hours, minutes)
}

/** "2026-11-25T09:00:00" fra API'et -> Date i lokal tid. */
export function fromLocalIso(value: string): Date {
  const [date, time = '00:00'] = value.split('T')
  return fromDateAndTime(date, time.slice(0, 5))
}

export function addMinutes(date: Date, minutes: number): Date {
  return new Date(date.getTime() + minutes * 60_000)
}
