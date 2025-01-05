'use client'

import { useRouter } from 'next/navigation'
import { PurchaseOrder } from '../data/mock-carts'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { ChevronRight } from 'lucide-react'

interface PurchaseOrderListProps {
  purchaseOrders: PurchaseOrder[];
}

export function PurchaseOrderList({ purchaseOrders }: PurchaseOrderListProps) {
  const router = useRouter()

  const handleRowClick = (orderId: string) => {
    router.push(`/purchase-orders/${orderId}`)
  }

  const handleRowMouseEnter = (orderId: string) => {
    router.prefetch(`/purchase-orders/${orderId}`)
  }

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-[100px]">Order ID</TableHead>
          <TableHead>Customer Name</TableHead>
          <TableHead>Cart ID</TableHead>
          <TableHead>Status</TableHead>
          <TableHead>Total Amount</TableHead>
          <TableHead className="hidden md:table-cell">Last Updated</TableHead>
          <TableHead className="w-4"></TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {purchaseOrders.map((order) => (
          <TableRow 
            key={order.id} 
            className="cursor-pointer hover:bg-gray-100 transition-colors"
            onClick={() => handleRowClick(order.id)}
            onMouseEnter={() => handleRowMouseEnter(order.id)}
          >
            <TableCell>
              <span>{`${order.id.slice(0, 2)}...${order.id.slice(-4)}`}</span>
            </TableCell>
            <TableCell>{order.customerName}</TableCell>
            <TableCell>{`${order.cartId.slice(0, 2)}...${order.cartId.slice(-4)}`}</TableCell>
            <TableCell>{order.status}</TableCell>
            <TableCell>${order.totalAmount.toFixed(2)}</TableCell>
            <TableCell className="hidden md:table-cell">{new Date(order.updatedAt).toLocaleString()}</TableCell>
            <TableCell className="w-4">
              <ChevronRight className="h-4 w-4 text-gray-400" />
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  )
}

