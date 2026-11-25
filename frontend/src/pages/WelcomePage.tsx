import { Link } from 'react-router-dom'

export default function WelcomePage() {
  return (
    <>
      <h1 className="auth-title">Velkommen til LevelUp</h1>
      <p className="muted">Her vælger brugeren mål og præferencer og opsætter sin første ugeplan.</p>
      <p className="auth-switch">
        <Link to="/">Gå til dashboard</Link>
      </p>
    </>
  )
}
