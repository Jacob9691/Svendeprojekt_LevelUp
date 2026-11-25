import { Link } from 'react-router-dom'

export default function RegisterPage() {
  return (
    <>
      <h1 className="auth-title">Opret konto</h1>
      <p className="muted">Registreringsformularen kommer her.</p>
      <p className="auth-switch">
        Har du allerede en konto? <Link to="/login">Log ind</Link>
      </p>
    </>
  )
}
