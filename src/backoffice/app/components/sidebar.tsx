'use client'

import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { BarChart, ShoppingCart, Package, X } from 'lucide-react'

type SidebarProps = {
  open: boolean
  setOpen: (open: boolean) => void
}

export default function Sidebar({ open, setOpen }: SidebarProps) {
  const pathname = usePathname()

  const links = [
    { href: '/dashboard', label: 'Dashboard', icon: BarChart },
    { href: '/carts', label: 'Carts', icon: ShoppingCart },
    { href: '/purchase-orders', label: 'Purchase Orders', icon: Package },
  ]

  return (
    <div
      className={`${
        open ? 'translate-x-0' : '-translate-x-full'
      } fixed inset-y-0 left-0 z-30 w-64 bg-gray-900 text-white transition duration-300 ease-in-out transform lg:translate-x-0 lg:static lg:inset-0`}
    >
      <div className="flex items-center justify-between h-16 px-6 bg-gray-800">
        <span className="text-xl font-semibold">Back Office</span>
        <button onClick={() => setOpen(false)} className="lg:hidden">
          <X className="w-6 h-6" />
        </button>
      </div>
      <nav className="px-4 py-6">
        <ul className="space-y-2">
          {links.map((link) => (
            <li key={link.href}>
              <Link
                href={link.href}
                className={`flex items-center px-4 py-2 rounded-lg hover:bg-gray-800 ${
                  pathname === link.href ? 'bg-gray-800' : ''
                }`}
              >
                <link.icon className="w-5 h-5 mr-3" />
                {link.label}
              </Link>
            </li>
          ))}
        </ul>
      </nav>
    </div>
  )
}

