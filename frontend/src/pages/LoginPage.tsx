import { useState, type FormEvent } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { login, resendTwoFactor, verifyTwoFactor } from '../api/auth'
import { getErrorCode, getErrorMessage } from '../api/errors'
import type { LoginResponse } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { Alert, Button, CodeField, TextField } from '../components/Form'

interface LoginState {
  email?: string
  confirmed?: boolean
}

/**
 * Login i to trin:
 * 1) e-mail og adgangskode → API'et sender en engangskode
 * 2) engangskoden → API'et returnerer et access token
 */
export default function LoginPage() {
  const { signIn } = useAuth()
  const state = (useLocation().state ?? {}) as LoginState

  const [email, setEmail] = useState(state.email ?? '')
  const [password, setPassword] = useState('')
  const [code, setCode] = useState('')
  const [challenge, setChallenge] = useState<LoginResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [needsConfirmation, setNeedsConfirmation] = useState(false)
  const [info, setInfo] = useState<string | null>(
    state.confirmed ? 'Din konto er bekræftet. Log ind for at fortsætte.' : null,
  )
  const [loading, setLoading] = useState(false)

  async function handlePassword(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setInfo(null)
    setNeedsConfirmation(false)
    setLoading(true)

    try {
      setChallenge(await login({ email, password }))
      setPassword('')
    } catch (err) {
      setNeedsConfirmation(getErrorCode(err) === 'accountNotConfirmed')
      setError(getErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  async function handleCode(event: FormEvent) {
    event.preventDefault()
    if (!challenge) return

    setError(null)
    setLoading(true)

    try {
      const response = await verifyTwoFactor(challenge.twoFactorToken, code)
      // GuestOnly sender brugeren videre, når de er logget ind
      signIn(response)
    } catch (err) {
      setCode('')
      setError(getErrorMessage(err))

      // 2FA-tokenet er udløbet (10 min) – start forfra med adgangskoden
      if (getErrorCode(err) === 'twoFactorTokenExpired') {
        setChallenge(null)
      }
    } finally {
      setLoading(false)
    }
  }

  async function handleResend() {
    if (!challenge) return

    setError(null)
    setLoading(true)

    try {
      const response = await resendTwoFactor(challenge.twoFactorToken)
      setChallenge(response)
      setInfo(`Vi har sendt en ny kode til ${response.sentTo}.`)
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  function startOver() {
    setChallenge(null)
    setCode('')
    setError(null)
    setInfo(null)
  }

  if (challenge) {
    const channel = challenge.method === 'Sms' ? 'SMS' : 'e-mail'

    return (
      <>
        <h1 className="h4 mb-3">Indtast din kode</h1>

        <form onSubmit={handleCode}>
          {error && <Alert>{error}</Alert>}
          <Alert kind="info">{info ?? `Vi har sendt en kode via ${channel} til ${challenge.sentTo}.`}</Alert>

          <CodeField value={code} onChange={(e) => setCode(e.target.value.replace(/\D/g, ''))} required autoFocus />

          <div className="d-grid gap-2">
            <Button type="submit" loading={loading} disabled={code.length !== 6}>
              Log ind
            </Button>
            <Button type="button" variant="link" onClick={handleResend} disabled={loading}>
              Send en ny kode
            </Button>
          </div>
        </form>

        <p className="mt-3 mb-0 small">
          <button type="button" className="btn btn-link btn-sm p-0" onClick={startOver}>
            Log ind med en anden konto
          </button>
        </p>
      </>
    )
  }

  return (
    <>
      <h1 className="h4 mb-3">Log ind</h1>

      <form onSubmit={handlePassword}>
        {info && <Alert kind="success">{info}</Alert>}
        {error && (
          <Alert>
            {error}
            {needsConfirmation && (
              <>
                {' '}
                <Link to="/bekraeft" state={{ email }} className="alert-link">
                  Bekræft konto
                </Link>
              </>
            )}
          </Alert>
        )}

        <TextField
          label="E-mail"
          type="email"
          autoComplete="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          autoFocus={!email}
        />
        <TextField
          label="Adgangskode"
          type="password"
          autoComplete="current-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          autoFocus={!!email}
        />

        <Button type="submit" loading={loading} className="w-100">
          Fortsæt
        </Button>
      </form>

      <p className="mt-3 mb-0 small">
        Har du ikke en konto? <Link to="/opret">Opret konto</Link>
      </p>
    </>
  )
}
