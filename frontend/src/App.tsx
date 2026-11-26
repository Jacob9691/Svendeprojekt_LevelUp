import { Route, Routes } from 'react-router-dom'
import { GuestOnly, RequireAuth } from './auth/RouteGuards'
import AppLayout from './layouts/AppLayout'
import AuthLayout from './layouts/AuthLayout'
import CharacterPage from './pages/CharacterPage'
import ConfirmAccountPage from './pages/ConfirmAccountPage'
import DashboardPage from './pages/DashboardPage'
import GoalsPage from './pages/GoalsPage'
import LoginPage from './pages/LoginPage'
import NotFoundPage from './pages/NotFoundPage'
import ProfilePage from './pages/ProfilePage'
import RegisterPage from './pages/RegisterPage'
import ShopPage from './pages/ShopPage'
import WeekPlanPage from './pages/WeekPlanPage'
import WelcomePage from './pages/WelcomePage'

export default function App() {
  return (
    <Routes>
      {/* Kræver login */}
      <Route element={<RequireAuth />}>
        <Route element={<AppLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="ugeplan" element={<WeekPlanPage />} />
          <Route path="maal" element={<GoalsPage />} />
          <Route path="shop" element={<ShopPage />} />
          <Route path="karakter" element={<CharacterPage />} />
          <Route path="profil" element={<ProfilePage />} />
        </Route>

        <Route element={<AuthLayout />}>
          <Route path="velkommen" element={<WelcomePage />} />
        </Route>
      </Route>

      {/* Kun for brugere der ikke er logget ind */}
      <Route element={<GuestOnly />}>
        <Route element={<AuthLayout />}>
          <Route path="login" element={<LoginPage />} />
          <Route path="opret" element={<RegisterPage />} />
          <Route path="bekraeft" element={<ConfirmAccountPage />} />
        </Route>
      </Route>

      <Route element={<AuthLayout />}>
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}
