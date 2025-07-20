"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Home } from "lucide-react";

export function Breadcrumbs() {
  const pathname = usePathname();

  if (pathname === "/") return null;

  const pathSegments = pathname.split("/").filter((segment) => segment !== "");

  return (
    <nav aria-label="Breadcrumb" className="mb-4">
      <ol className="flex items-center space-x-2 text-sm text-gray-500">
        <li>
          <Link href="/" className="flex items-center hover:text-gray-700">
            <Home className="w-4 h-4 mr-1" />
            <span className="sr-only">Home</span>
          </Link>
        </li>
        {pathSegments.map((segment, index) => {
          const href = `/${pathSegments.slice(0, index + 1).join("/")}`;
          const isLast = index === pathSegments.length - 1;

          return (
            <li key={segment} className="flex items-center">
              <span className="mx-2">/</span>
              {isLast ? (
                <span className="font-medium text-gray-900">{segment}</span>
              ) : (
                <Link href={href} className="hover:text-gray-700">
                  {segment}
                </Link>
              )}
            </li>
          );
        })}
      </ol>
    </nav>
  );
}
