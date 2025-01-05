"use client";

import { Cart } from "@/lib/monostore-api/monostore-api";
import { use, useState } from "react";
import { CartFilter } from "../../components/cart-filter";
import { CartList } from "../../components/cart-list";

type CartsContentProps = {
  carts: Promise<Cart[]>;
};

export default function CartsContent({ carts }: CartsContentProps) {
  const resolvedCarts = use(carts);
  const [isExpanded, setIsExpanded] = useState(false);
  const [filters, setFilters] = useState({
    country: "",
    cartId: "",
    sku: "",
    userId: "",
    sessionId: "",
  });

  const handleFilterChange = (key: string, value: string) => {
    setFilters((prev) => ({ ...prev, [key]: value }));
  };

  const handleFilterSubmit = () => {
    //   const filtered = mockCarts.filter((cart) => {
    //     if (
    //       filters.country &&
    //       filters.country !== "all" &&
    //       cart.country !== filters.country
    //     )
    //       return false;
    //     if (
    //       filters.cartId &&
    //       !cart.shortId.toLowerCase().includes(filters.cartId.toLowerCase())
    //     )
    //       return false;
    //     if (
    //       filters.sku &&
    //       !cart.items.some((item) =>
    //         item.sku.toLowerCase().includes(filters.sku.toLowerCase())
    //       )
    //     )
    //       return false;
    //     if (
    //       filters.userId &&
    //       cart.userId?.toLowerCase() !== filters.userId.toLowerCase()
    //     )
    //       return false;
    //     if (
    //       filters.sessionId &&
    //       cart.sessionId.toLowerCase() !== filters.sessionId.toLowerCase()
    //     )
    //       return false;
    //     return true;
    //   });
    //   setFilteredCarts(filtered);
  };

  return (
    <div className="space-y-4">
      <CartFilter
        filters={filters}
        onFilterChange={handleFilterChange}
        onFilterSubmit={handleFilterSubmit}
        isExpanded={isExpanded}
        onToggleExpand={() => setIsExpanded(!isExpanded)}
      />
      <CartList carts={resolvedCarts} />
    </div>
  );
}
