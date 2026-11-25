import { Link } from 'react-router-dom'

export default function LoginPage() {
  return (
    <>
      <h1 className="auth-title">Log ind</h1>
      <p className="muted">Login-formularen kommer her.</p>
      <p className="auth-switch">
        Har du ikke en konto? <Link to="/opret">Opret konto</Link>
      </p>
    </>
  )
}
