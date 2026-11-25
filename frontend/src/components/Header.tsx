import { useState } from 'react'
import { Link, NavLink } from 'react-router-dom'
import { navItems } from '../navigation'
import { CloseIcon, CoinIcon, MenuIcon } from './Icons'

// Midlertidige data indtil brugeren hentes fra API'et.
const placeholderUser = {
  displayName: 'Spiller',
  level: 1,
  xp: 40,
  xpForNextLevel: 100,
  coins: 0,
}

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false)
  const { displayName, level, xp, xpForNextLevel, coins } = placeholderUser
  const progress = Math.min(100, Math.round((xp / xpForNextLevel) * 100))

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
        </nav>

        <div className="header__stats">
          <div className="level" title={`${xp}/${xpForNextLevel} XP`}>
            <span className="level__badge">Lv {level}</span>
            <div
              className="level__bar"
              role="progressbar"
              aria-label="XP"
              aria-valuemin={0}
              aria-valuemax={xpForNextLevel}
              aria-valuenow={xp}
            >
              <div className="level__fill" style={{ width: `${progress}%` }} />
            </div>
          </div>

          <div className="coins" title="Points">
            <CoinIcon />
            <span>{coins}</span>
          </div>

          <Link to="/profil" className="avatar" title={displayName}>
            {displayName.charAt(0)}
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
