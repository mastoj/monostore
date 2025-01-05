"use client";

import { PurchaseOrderList } from "@/components/purchase-order-list";
import { useState } from "react";
import { PurchaseOrderFilter } from "../../components/purchase-order-filter";
import { mockPurchaseOrders, PurchaseOrder } from "../../data/mock-carts";

export default function PurchaseOrdersContent() {
  const [filteredOrders, setFilteredOrders] =
    useState<PurchaseOrder[]>(mockPurchaseOrders);
  const [isExpanded, setIsExpanded] = useState(false);
  const [filters, setFilters] = useState({
    status: "",
    orderId: "",
    cartId: "",
    customerName: "",
  });

  const handleFilterChange = (key: string, value: string) => {
    setFilters((prev) => ({ ...prev, [key]: value }));
  };

  const handleFilterSubmit = () => {
    const filtered = mockPurchaseOrders.filter((order) => {
      if (
        filters.status &&
        filters.status !== "all" &&
        order.status !== filters.status
      )
        return false;
      if (
        filters.orderId &&
        !order.id.toLowerCase().includes(filters.orderId.toLowerCase())
      )
        return false;
      if (
        filters.cartId &&
        !order.cartId.toLowerCase().includes(filters.cartId.toLowerCase())
      )
        return false;
      if (
        filters.customerName &&
        !order.customerName
          .toLowerCase()
          .includes(filters.customerName.toLowerCase())
      )
        return false;
      return true;
    });
    setFilteredOrders(filtered);
  };

  return (
    <div className="space-y-4">
      <PurchaseOrderFilter
        filters={filters}
        onFilterChange={handleFilterChange}
        onFilterSubmit={handleFilterSubmit}
        isExpanded={isExpanded}
        onToggleExpand={() => setIsExpanded(!isExpanded)}
      />
      <PurchaseOrderList purchaseOrders={filteredOrders} />
    </div>
  );
}
