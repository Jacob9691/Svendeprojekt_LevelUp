import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <>
      <h1 className="h4 mb-3">Siden findes ikke</h1>
      <Link to="/" className="btn btn-primary w-100">
        Tilbage til forsiden
      </Link>
    </>
  )
}
