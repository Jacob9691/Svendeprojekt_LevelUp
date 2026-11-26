import { Outlet } from 'react-router-dom'

/** Layout for login, registrering og onboarding – uden menu. */
export default function AuthLayout() {
  return (
    <div className="bg-body-tertiary min-vh-100 d-flex align-items-center py-4">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-12 col-sm-10 col-md-7 col-lg-5 col-xl-4">
            <div className="card shadow-sm">
              <div className="card-body p-4">
                <div className="text-center fs-3 fw-bold mb-3">
                  Level<span className="text-primary">Up</span>
                </div>
                <Outlet />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
