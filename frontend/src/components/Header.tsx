import { useState } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'
import { navItems } from '../navigation'

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false)
  const { profile, signOut } = useAuth()

  const xpPercent = profile
    ? Math.min(100, Math.round((profile.xpIntoLevel / profile.xpForNextLevel) * 100))
    : 0

  return (
    <nav className="navbar navbar-expand-md bg-body-tertiary border-bottom sticky-top">
      <div className="container">
        <Link to="/" className="navbar-brand fw-bold">
          Level<span className="text-primary">Up</span>
        </Link>

        <button
          type="button"
          className="navbar-toggler"
          aria-label={menuOpen ? 'Luk menu' : 'Åbn menu'}
          aria-expanded={menuOpen}
          aria-controls="main-nav"
          onClick={() => setMenuOpen((open) => !open)}
        >
          <span className="navbar-toggler-icon" />
        </button>

        <div id="main-nav" className={`collapse navbar-collapse${menuOpen ? ' show' : ''}`}>
          <ul className="navbar-nav me-auto">
            {navItems.map(({ to, label }) => (
              <li key={to} className="nav-item">
                <NavLink to={to} end={to === '/'} className="nav-link" onClick={() => setMenuOpen(false)}>
                  {label}
                </NavLink>
              </li>
            ))}
          </ul>

          {profile && (
            <div className="d-flex align-items-center gap-3 my-2 my-md-0 me-md-3">
              <span className="badge text-bg-primary">Level {profile.level}</span>
              <div
                className="progress"
                style={{ width: 100, height: 8 }}
                role="progressbar"
                aria-label="XP"
                aria-valuemin={0}
                aria-valuemax={profile.xpForNextLevel}
                aria-valuenow={profile.xpIntoLevel}
                title={`${profile.xpIntoLevel}/${profile.xpForNextLevel} XP`}
              >
                <div className="progress-bar bg-success" style={{ width: `${xpPercent}%` }} />
              </div>
              <span className="badge text-bg-warning">{profile.coinBalance} points</span>
            </div>
          )}

          <button type="button" className="btn btn-outline-secondary btn-sm" onClick={signOut}>
            Log ud
          </button>
        </div>
      </div>
    </nav>
  )
}
