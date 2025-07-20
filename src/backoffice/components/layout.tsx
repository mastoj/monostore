"use client";

import { ReactNode, Suspense, useState } from "react";
import { Breadcrumbs } from "./breadcrumbs";
import Header from "./header";
import Sidebar from "./sidebar";

export default function Layout({ children }: { children: ReactNode }) {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className="flex h-screen bg-gray-100">
      <Suspense>
        <Sidebar open={sidebarOpen} setOpen={setSidebarOpen} />
      </Suspense>
      <div className="flex-1 flex flex-col overflow-hidden">
        <Header sidebarOpen={sidebarOpen} setSidebarOpen={setSidebarOpen} />
        <main className="flex-1 overflow-x-hidden overflow-y-auto bg-gray-200 p-4 md:p-6 lg:p-8">
          <Suspense>
            <Breadcrumbs />
          </Suspense>
          {children}
        </main>
      </div>
    </div>
  );
}
