'use client'

import { Menu } from 'lucide-react'

type HeaderProps = {
  sidebarOpen: boolean
  setSidebarOpen: (open: boolean) => void
}

export default function Header({ sidebarOpen, setSidebarOpen }: HeaderProps) {
  return (
    <header className="bg-white shadow-sm">
      <div className="flex items-center justify-between h-16 px-6">
        <button
          onClick={() => setSidebarOpen(!sidebarOpen)}
          className="text-gray-500 focus:outline-none focus:text-gray-700 lg:hidden"
        >
          <Menu className="w-6 h-6" />
        </button>
        <div className="flex items-center">
          <span className="text-xl font-semibold text-gray-800">E-commerce Admin</span>
        </div>
        <div className="flex items-center">
          {/* Add user profile, notifications, etc. here */}
        </div>
      </div>
    </header>
  )
}

