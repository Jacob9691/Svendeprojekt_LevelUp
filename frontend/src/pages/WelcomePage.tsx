import { Link } from 'react-router-dom'

export default function WelcomePage() {
  return (
    <>
      <h1 className="h4 mb-3">Velkommen til LevelUp</h1>
      <p className="text-body-secondary">
        Her vælger brugeren mål og præferencer og opsætter sin første ugeplan.
      </p>
      <Link to="/" className="btn btn-primary w-100">
        Gå til dashboard
      </Link>
    </>
  )
}
