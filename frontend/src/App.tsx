import { Route, Routes } from 'react-router-dom'
import AppLayout from './layouts/AppLayout'
import AuthLayout from './layouts/AuthLayout'
import CharacterPage from './pages/CharacterPage'
import DashboardPage from './pages/DashboardPage'
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
      <Route element={<AppLayout />}>
        <Route index element={<DashboardPage />} />
        <Route path="ugeplan" element={<WeekPlanPage />} />
        <Route path="shop" element={<ShopPage />} />
        <Route path="karakter" element={<CharacterPage />} />
        <Route path="profil" element={<ProfilePage />} />
      </Route>

      <Route element={<AuthLayout />}>
        <Route path="login" element={<LoginPage />} />
        <Route path="opret" element={<RegisterPage />} />
        <Route path="velkommen" element={<WelcomePage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}
