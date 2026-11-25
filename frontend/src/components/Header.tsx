import { useState } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'
import { navItems } from '../navigation'
import { CloseIcon, CoinIcon, MenuIcon } from './Icons'

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false)
  const { user, profile, signOut } = useAuth()

  const displayName = profile?.displayName ?? user?.displayName ?? ''
  const progress = profile
    ? Math.min(100, Math.round((profile.xpIntoLevel / profile.xpForNextLevel) * 100))
    : 0

  return (
    <header className="header">
      <div className="container header__inner">
        <Link to="/" className="header__brand">
          Level<span>Up</span>
        </Link>

        <nav
          id="main-nav"
          className={`header__nav${menuOpen ? ' is-open' : ''}`}
          aria-label="Hovedmenu"
        >
          {navItems.map(({ to, label }) => (
            <NavLink
              key={to}
              to={to}
              end={to === '/'}
              className={({ isActive }) => `header__link${isActive ? ' is-active' : ''}`}
              onClick={() => setMenuOpen(false)}
            >
              {label}
            </NavLink>
          ))}
          <button type="button" className="header__link header__logout" onClick={signOut}>
            Log ud
          </button>
        </nav>

        <div className="header__stats">
          {profile && (
            <>
              <div className="level" title={`${profile.xpIntoLevel}/${profile.xpForNextLevel} XP`}>
                <span className="level__badge">Lv {profile.level}</span>
                <div
                  className="level__bar"
                  role="progressbar"
                  aria-label="XP"
                  aria-valuemin={0}
                  aria-valuemax={profile.xpForNextLevel}
                  aria-valuenow={profile.xpIntoLevel}
                >
                  <div className="level__fill" style={{ width: `${progress}%` }} />
                </div>
              </div>

              <div className="coins" title="Points">
                <CoinIcon />
                <span>{profile.coinBalance}</span>
              </div>
            </>
          )}

          <Link to="/profil" className="avatar" title={displayName}>
            {displayName.charAt(0).toUpperCase()}
          </Link>

          <button
            type="button"
            className="header__toggle"
            aria-label={menuOpen ? 'Luk menu' : 'Åbn menu'}
            aria-expanded={menuOpen}
            aria-controls="main-nav"
            onClick={() => setMenuOpen((open) => !open)}
          >
            {menuOpen ? <CloseIcon /> : <MenuIcon />}
          </button>
        </div>
      </div>
    </header>
  )
}
