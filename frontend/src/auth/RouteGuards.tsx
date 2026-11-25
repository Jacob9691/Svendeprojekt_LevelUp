import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from './useAuth'

/** Sider der kræver login. Sender brugeren til /login og husker hvor de kom fra. */
export function RequireAuth() {
  const { isAuthenticated } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  return <Outlet />
}

/**
 * Login- og opret-sider. En bruger der er logget ind sendes videre – til den side de prøvede
 * at åbne før login (sat af RequireAuth), ellers til forsiden. Det er også her brugeren
 * sendes videre lige efter login.
 */
export function GuestOnly() {
  const { isAuthenticated } = useAuth()
  const location = useLocation()

  if (isAuthenticated) {
    const from = (location.state as { from?: string } | null)?.from ?? '/'
    return <Navigate to={from} replace />
  }

  return <Outlet />
}
