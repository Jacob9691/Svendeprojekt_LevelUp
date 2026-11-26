import { useId, type ButtonHTMLAttributes, type InputHTMLAttributes, type ReactNode } from 'react'

interface TextFieldProps extends InputHTMLAttributes<HTMLInputElement> {
  label: string
  error?: string
  hint?: string
}

/** Bootstrap-inputfelt med label, hjælpetekst og fejlbesked. */
export function TextField({ label, error, hint, className, ...inputProps }: TextFieldProps) {
  const id = useId()
  const messageId = `${id}-message`

  return (
    <div className="mb-3">
      <label htmlFor={id} className="form-label">
        {label}
      </label>
      <input
        id={id}
        className={`form-control${error ? ' is-invalid' : ''}${className ? ` ${className}` : ''}`}
        aria-describedby={error || hint ? messageId : undefined}
        {...inputProps}
      />
      {error ? (
        <div id={messageId} className="invalid-feedback">
          {error}
        </div>
      ) : (
        hint && (
          <div id={messageId} className="form-text">
            {hint}
          </div>
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
      className="form-control-lg text-center font-monospace"
      {...props}
    />
  )
}

interface RadioGroupProps<T extends string> {
  legend: string
  name: string
  value: T
  options: { value: T; label: string }[]
  onChange: (value: T) => void
}

export function RadioGroup<T extends string>({ legend, name, value, options, onChange }: RadioGroupProps<T>) {
  return (
    <fieldset className="mb-3">
      <legend className="form-label fs-6">{legend}</legend>
      {options.map((option) => (
        <div key={option.value} className="form-check form-check-inline">
          <input
            id={`${name}-${option.value}`}
            className="form-check-input"
            type="radio"
            name={name}
            checked={value === option.value}
            onChange={() => onChange(option.value)}
          />
          <label htmlFor={`${name}-${option.value}`} className="form-check-label">
            {option.label}
          </label>
        </div>
      ))}
    </fieldset>
  )
}

const alertClass = { error: 'alert-danger', info: 'alert-info', success: 'alert-success' }

export function Alert({ kind = 'error', children }: { kind?: 'error' | 'info' | 'success'; children: ReactNode }) {
  return (
    <div className={`alert ${alertClass[kind]} py-2`} role={kind === 'error' ? 'alert' : 'status'}>
      {children}
    </div>
  )
}

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'outline-secondary' | 'link'
  loading?: boolean
}

export function Button({ variant = 'primary', loading, disabled, children, className, ...props }: ButtonProps) {
  return (
    <button
      className={`btn btn-${variant}${className ? ` ${className}` : ''}`}
      disabled={disabled || loading}
      {...props}
    >
      {loading && <span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />}
      {children}
    </button>
  )
}
