import { useState, type FormEvent } from 'react'
import { getErrorMessage, getFieldErrors } from '../../api/errors'
import { confirmPhoneChange, requestPhoneChange } from '../../api/profile'
import type { Settings } from '../../api/types'
import { Alert, Button, CodeField, TextField } from '../Form'

interface PhoneCardProps {
  settings: Settings
  onConfirmed: (settings: Settings) => void
}

/**
 * Tilføj eller skift telefonnummer i to trin: send kode -> bekræft kode.
 * Nummeret gemmes først, når koden er bekræftet.
 */
export default function PhoneCard({ settings, onConfirmed }: PhoneCardProps) {
  const [editing, setEditing] = useState(false)
  const [phoneNumber, setPhoneNumber] = useState('')
  const [code, setCode] = useState('')
  const [codeSent, setCodeSent] = useState(false)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  async function handleSendCode(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setBusy(true)

    try {
      await requestPhoneChange(phoneNumber)
      setCodeSent(true)
    } catch (err) {
      const fieldErrors = getFieldErrors(err)
      setError(fieldErrors.phoneNumber ?? getErrorMessage(err))
    } finally {
      setBusy(false)
    }
  }

  async function handleConfirm(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setBusy(true)

    try {
      onConfirmed(await confirmPhoneChange(phoneNumber, code))
      setSuccess('Telefonnummeret er bekræftet.')
      cancel()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setBusy(false)
    }
  }

  function cancel() {
    setEditing(false)
    setCodeSent(false)
    setPhoneNumber('')
    setCode('')
    setError(null)
  }

  return (
    <div className="card h-100">
      <div className="card-body">
        <h2 className="h6 card-title mb-3">Telefonnummer</h2>

        {success && <Alert kind="success">{success}</Alert>}
        {error && <Alert>{error}</Alert>}

        {settings.phoneNumber ? (
          <p className="mb-3">
            {settings.phoneNumber}{' '}
            {settings.phoneNumberConfirmed ? (
              <span className="badge text-bg-success">Bekræftet</span>
            ) : (
              <span className="badge text-bg-secondary">Ikke bekræftet</span>
            )}
          </p>
        ) : (
          <p className="text-body-secondary mb-3">Du har ikke tilføjet et telefonnummer.</p>
        )}

        {!editing && (
          <Button
            variant="outline-secondary"
            className="btn-sm"
            onClick={() => {
              setSuccess(null)
              setEditing(true)
            }}
          >
            {settings.phoneNumber ? 'Skift telefonnummer' : 'Tilføj telefonnummer'}
          </Button>
        )}

        {editing && !codeSent && (
          <form onSubmit={handleSendCode}>
            <TextField
              label="Nyt telefonnummer"
              type="tel"
              autoComplete="tel"
              placeholder="+4512345678"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              required
              autoFocus
            />
            <div className="d-flex gap-2">
              <Button type="submit" className="btn-sm" loading={busy} disabled={!phoneNumber.trim()}>
                Send kode
              </Button>
              <Button type="button" variant="link" className="btn-sm" onClick={cancel}>
                Annuller
              </Button>
            </div>
          </form>
        )}

        {editing && codeSent && (
          <form onSubmit={handleConfirm}>
            <Alert kind="info">Vi har sendt en kode til {phoneNumber}.</Alert>
            <CodeField value={code} onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))} required autoFocus />
            <div className="d-flex gap-2">
              <Button type="submit" className="btn-sm" loading={busy} disabled={code.length !== 6}>
                Bekræft
              </Button>
              <Button type="button" variant="link" className="btn-sm" onClick={cancel}>
                Annuller
              </Button>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}
