import { Outlet, useLocation } from 'react-router-dom'
import Footer from '../components/Footer'
import Header from '../components/Header'
import { navItems } from '../navigation'

/** Layout for sider, hvor brugeren er logget ind. */
export default function AppLayout() {
  const { pathname } = useLocation()
  const title = navItems.find((item) => item.to === pathname)?.label

  return (
    <div className="d-flex flex-column min-vh-100">
      <Header />
      <main className="container py-4">
        {title && <h1 className="h3 mb-4">{title}</h1>}
        <Outlet />
      </main>
      <Footer />
    </div>
  )
}
