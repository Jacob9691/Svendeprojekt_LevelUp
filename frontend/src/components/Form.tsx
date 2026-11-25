import { useId, type ButtonHTMLAttributes, type InputHTMLAttributes, type ReactNode } from 'react'

interface TextFieldProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string
  error?: string
  hint?: string
}

/** Inputfelt med label, hjælpetekst og fejlbesked. */
export function TextField({ label, error, hint, ...inputProps }: TextFieldProps) {
  const id = useId()
  const messageId = `${id}-message`

  return (
    <div className="field">
      <label htmlFor={id} className="field__label">
        {label}
      </label>
      <input
        id={id}
        className={`field__input${error ? ' has-error' : ''}`}
        aria-invalid={error ? true : undefined}
        aria-describedby={error || hint ? messageId : undefined}
        {...inputProps}
      />
      {error ? (
        <p id={messageId} className="field__error">
          {error}
        </p>
      ) : (
        hint && (
          <p id={messageId} className="field__hint">
            {hint}
          </p>
        )
      )}
    </div>
  )
}

/** Felt til 6-cifrede engangskoder. Browsere på mobil kan udfylde koden fra SMS'en. */
export function CodeField(props: Omit<TextFieldProps, 'label'> & { label?: string }) {
  return (
    <TextField
      label="Kode"
      inputMode="numeric"
      autoComplete="one-time-code"
      pattern="\d{6}"
      maxLength={6}
      placeholder="123456"
      className="field__input field__input--code"
      {...props}
    />
  )
}

export function Alert({ kind = 'error', children }: { kind?: 'error' | 'info' | 'success'; children: ReactNode }) {
  return (
    <div className={`alert alert--${kind}`} role={kind === 'error' ? 'alert' : 'status'}>
      {children}
    </div>
  )
}

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'link'
  loading?: boolean
}

export function Button({ variant = 'primary', loading, disabled, children, className, ...props }: ButtonProps) {
  return (
    <button
      className={`button button--${variant}${className ? ` ${className}` : ''}`}
      disabled={disabled || loading}
      aria-busy={loading || undefined}
      {...props}
    >
      {loading ? 'Vent…' : children}
    </button>
  )
}
