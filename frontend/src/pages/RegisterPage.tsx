import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { register } from '../api/auth'
import { getErrorMessage, getFieldErrors } from '../api/errors'
import type { TwoFactorMethod } from '../api/types'
import { Alert, Button, TextField } from '../components/Form'

export default function RegisterPage() {
  const navigate = useNavigate()
  const [displayName, setDisplayName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [method, setMethod] = useState<TwoFactorMethod>('Email')
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({})
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setFieldErrors({})
    setLoading(true)

    try {
      const response = await register({
        email,
        password,
        displayName,
        phoneNumber: phoneNumber.trim() || undefined,
        confirmationMethod: method,
      })

      navigate('/bekraeft', {
        state: { email, method: response.confirmationMethod, sentTo: response.sentTo },
      })
    } catch (err) {
      const errors = getFieldErrors(err)
      setFieldErrors(errors)

      // Fejl der ikke hører til et bestemt felt vises øverst
      if (Object.keys(errors).length === 0) {
        setError(getErrorMessage(err))
      } else if (errors['']) {
        setError(errors[''])
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <>
      <h1 className="auth-title">Opret konto</h1>

      <form className="form" onSubmit={handleSubmit} noValidate>
        {error && <Alert>{error}</Alert>}

        <TextField
          label="Navn"
          autoComplete="nickname"
          value={displayName}
          onChange={(e) => setDisplayName(e.target.value)}
          error={fieldErrors.displayName}
          required
        />
        <TextField
          label="E-mail"
          type="email"
          autoComplete="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          error={fieldErrors.email}
          required
        />
        <TextField
          label="Adgangskode"
          type="password"
          autoComplete="new-password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          error={fieldErrors.password}
          hint="Mindst 8 tegn med stort og lille bogstav og et tal."
          required
        />

        <fieldset className="choice">
          <legend className="field__label">Send bekræftelseskode via</legend>
          <label className="choice__option">
            <input type="radio" name="method" checked={method === 'Email'} onChange={() => setMethod('Email')} />
            E-mail
          </label>
          <label className="choice__option">
            <input type="radio" name="method" checked={method === 'Sms'} onChange={() => setMethod('Sms')} />
            SMS
          </label>
        </fieldset>

        {method === 'Sms' && (
          <TextField
            label="Telefonnummer"
            type="tel"
            autoComplete="tel"
            placeholder="+4512345678"
            value={phoneNumber}
            onChange={(e) => setPhoneNumber(e.target.value)}
            error={fieldErrors.phoneNumber}
            required
          />
        )}

        <Button type="submit" loading={loading}>
          Opret konto
        </Button>
      </form>

      <p className="auth-switch">
        Har du allerede en konto? <Link to="/login">Log ind</Link>
      </p>
    </>
  )
}
