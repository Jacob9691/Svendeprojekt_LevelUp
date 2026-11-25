import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <>
      <h1 className="auth-title">Siden findes ikke</h1>
      <p className="auth-switch">
        <Link to="/">Tilbage til forsiden</Link>
      </p>
    </>
  )
}
