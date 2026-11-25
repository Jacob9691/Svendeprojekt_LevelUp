import { Outlet } from 'react-router-dom'

/** Layout for login, registrering og onboarding – uden menu. */
export default function AuthLayout() {
  return (
    <div className="auth-layout">
      <div className="auth-layout__card">
        <div className="auth-layout__brand">
          Level<span>Up</span>
        </div>
        <Outlet />
      </div>
    </div>
  )
}
