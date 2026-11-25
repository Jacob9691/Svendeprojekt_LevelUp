export interface NavItem {
  to: string
  label: string
}

export const navItems: NavItem[] = [
  { to: '/', label: 'Dashboard' },
  { to: '/ugeplan', label: 'Ugeplan' },
  { to: '/shop', label: 'Shop' },
  { to: '/karakter', label: 'Karakter' },
  { to: '/profil', label: 'Profil' },
]
