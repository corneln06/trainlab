import { NavLink, Outlet } from 'react-router'
import HealthBadge from '../components/api/HealthBadge'
import ThemeToggle from '../components/ThemeToggler'


const links = [
  { to: '/', label: 'Dashboard' },
  { to: '/activities', label: 'Activities' },
  { to: '/import', label: 'Import' },
  { to: '/analytics', label: 'Analytics' },
  { to: '/heatmap', label: 'Heatmap' },
  { to: '/settings', label: 'Settings' }
]

export default function AppLayout(){
    return (
        <div className="drawer lg:drawer-open">
            <input id="nav-drawer" type="checkbox" className="drawer-toggle" />

            <div className="drawer-content flex min-h-screen flex-col">
        <header className="navbar border-b border-base-300 bg-base-100">
          <label htmlFor="nav-drawer" className="btn btn-ghost btn-square lg:hidden" aria-label="Open menu">
            ☰
          </label>
          <span className="flex-1 px-2 text-lg font-semibold">TrainLab</span>
          <HealthBadge />
          <ThemeToggle />
        </header>

        <main className="flex-1 p-6">
          <Outlet />
        </main>
      </div>

      <div className="drawer-side">
        <label htmlFor="nav-drawer" className="drawer-overlay" aria-label="Close menu" />
        <ul className="menu min-h-full w-60 bg-base-200 p-4">
          {links.map((link) => (
            <li key={link.to}>
              <NavLink
                to={link.to}
                end={link.to === '/'}
                className={({ isActive }) => (isActive ? 'menu-active' : '')}
              >
                {link.label}
              </NavLink>
            </li>
          ))}
        </ul>
      </div>
    </div>
    )
}