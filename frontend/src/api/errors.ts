import { isAxiosError } from 'axios'
import type { ProblemDetails } from './types'

function getProblem(error: unknown): ProblemDetails | undefined {
  return isAxiosError<ProblemDetails>(error) ? error.response?.data : undefined
}

/** Den faste fejlkode fra API'et, fx. "accountNotConfirmed" eller "lockedOut". */
export function getErrorCode(error: unknown): string | undefined {
  return getProblem(error)?.code
}

/** Valideringsfejl pr. felt. Nøglerne gøres små (Email -> email), så de matcher formularens felter. */
export function getFieldErrors(error: unknown): Record<string, string> {
  const errors = getProblem(error)?.errors ?? {}
  const result: Record<string, string> = {}

  for (const [field, messages] of Object.entries(errors)) {
    const key = field ? field.charAt(0).toLowerCase() + field.slice(1) : ''
    result[key] = messages.join(' ')
  }

  return result
}

/** En besked der kan vises til brugeren. */
export function getErrorMessage(error: unknown): string {
  if (isAxiosError(error)) {
    if (!error.response) {
      return 'Kunne ikke få forbindelse til serveren. Prøv igen om lidt.'
    }

    if (error.response.status === 429) {
      return 'Du har prøvet for mange gange. Vent et minut og prøv igen.'
    }
  }

  const problem = getProblem(error)
  const fieldErrors = Object.values(problem?.errors ?? {}).flat()

  // Brug feltfejlen når der kun er én – den er mere præcis end den generelle titel
  if (fieldErrors.length === 1) {
    return fieldErrors[0]
  }

  return problem?.detail ?? problem?.title ?? 'Der skete en uventet fejl.'
}
