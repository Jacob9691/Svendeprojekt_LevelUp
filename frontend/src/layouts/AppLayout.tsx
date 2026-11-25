import { Outlet, useLocation } from 'react-router-dom'
import Footer from '../components/Footer'
import Header from '../components/Header'
import { navItems } from '../navigation'

/** Layout for sider, hvor brugeren er logget ind. */
export default function AppLayout() {
  const { pathname } = useLocation()
  const title = navItems.find((item) => item.to === pathname)?.label

  return (
    <div className="app-layout">
      <Header />
      <main className="app-layout__content container">
        {title && <h1 className="page-title">{title}</h1>}
        <Outlet />
      </main>
      <Footer />
    </div>
  )
}
