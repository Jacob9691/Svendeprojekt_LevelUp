export interface NavItem {
  to: string
  label: string
}

export const navItems: NavItem[] = [
  { to: '/', label: 'Dashboard' },
  { to: '/week-plan', label: 'Ugeplan' },
  { to: '/goals', label: 'Mål' },
  { to: '/shop', label: 'Shop' },
  { to: '/character', label: 'Karakter' },
  { to: '/profile', label: 'Profil' },
]
