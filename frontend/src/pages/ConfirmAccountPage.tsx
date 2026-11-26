import { useState, type FormEvent } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { confirmAccount, resendConfirmation } from '../api/auth'
import { getErrorMessage } from '../api/errors'
import type { TwoFactorMethod } from '../api/types'
import { Alert, Button, CodeField, RadioGroup, TextField } from '../components/Form'

interface ConfirmState {
  email?: string
  method?: TwoFactorMethod
  sentTo?: string
}

/** Bekræft konto med koden fra e-mail eller SMS. Kommer man fra "Opret konto", er e-mailen udfyldt. */
export default function ConfirmAccountPage() {
  const navigate = useNavigate()
  const state = (useLocation().state ?? {}) as ConfirmState

  const [email, setEmail] = useState(state.email ?? '')
  const [method, setMethod] = useState<TwoFactorMethod>(state.method ?? 'Email')
  const [code, setCode] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [info, setInfo] = useState<string | null>(
    state.sentTo ? `Vi har sendt en kode til ${state.sentTo}.` : null,
  )
  const [loading, setLoading] = useState(false)
  const [resending, setResending] = useState(false)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setLoading(true)

    try {
      await confirmAccount({ email, code, method })
      navigate('/login', { replace: true, state: { email, confirmed: true } })
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  async function handleResend() {
    setError(null)
    setResending(true)

    try {
      const response = await resendConfirmation(email, method)
      setInfo(response.message)
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setResending(false)
    }
  }

  return (
    <>
      <h1 className="h4 mb-3">Bekræft din konto</h1>

      <form onSubmit={handleSubmit}>
        {error && <Alert>{error}</Alert>}
        {info && !error && <Alert kind="info">{info}</Alert>}

        {!state.email && (
          <TextField
            label="E-mail"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        )}

        {!state.method && (
          <RadioGroup
            legend="Koden blev sendt via"
            name="method"
            value={method}
            options={[
              { value: 'Email', label: 'E-mail' },
              { value: 'Sms', label: 'SMS' },
            ]}
            onChange={setMethod}
          />
        )}

        <CodeField value={code} onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))} required autoFocus />

        <div className="d-grid gap-2">
          <Button type="submit" loading={loading} disabled={code.length !== 6 || !email}>
            Bekræft
          </Button>
          <Button type="button" variant="link" onClick={handleResend} loading={resending} disabled={!email}>
            Send en ny kode
          </Button>
        </div>
      </form>

      <p className="mt-3 mb-0 small">
        <Link to="/login">Tilbage til login</Link>
      </p>
    </>
  )
}
