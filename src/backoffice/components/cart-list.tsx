"use client";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Cart } from "@/lib/monostore-api/monostore-api";
import { ChevronRight } from "lucide-react";
import { useRouter } from "next/navigation";

interface CartListProps {
  carts: Cart[];
}

export function CartList({ carts }: CartListProps) {
  const router = useRouter();

  const handleRowClick = (cartId: string) => {
    router.push(`/carts/${cartId}`);
  };

  const handleRowMouseEnter = (cartId: string) => {
    router.prefetch(`/carts/${cartId}`);
  };

  return (
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead className="w-[100px]">Cart ID</TableHead>
          <TableHead className="hidden md:table-cell">Country</TableHead>
          <TableHead className="hidden md:table-cell">User ID</TableHead>
          <TableHead className="hidden md:table-cell">Session ID</TableHead>
          <TableHead>Items</TableHead>
          <TableHead>Total</TableHead>
          {/* <TableHead className="hidden md:table-cell">Created At</TableHead> */}
          <TableHead className="w-4"></TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {carts.map((cart) => (
          <TableRow
            key={cart.id}
            className="cursor-pointer hover:bg-gray-100 transition-colors"
            onClick={() => handleRowClick(cart.id)}
            onMouseEnter={() => handleRowMouseEnter(cart.id)}
          >
            <TableCell>
              <span>{`${cart.id.slice(0, 2)}...${cart.id.slice(-4)}`}</span>
            </TableCell>
            <TableCell className="hidden md:table-cell">
              {cart.operatingChain}
            </TableCell>
            <TableCell className="hidden md:table-cell">
              {cart.userId || "N/A"}
            </TableCell>
            <TableCell className="hidden md:table-cell">
              {cart.sessionId}
            </TableCell>
            <TableCell>{cart.items.length}</TableCell>
            <TableCell>
              $
              {cart.items
                .reduce(
                  (total, item) => total + item.product.price * item.quantity,
                  0
                )
                .toFixed(2)}
            </TableCell>
            {/* <TableCell className="hidden md:table-cell">
              {new Date(cart.createdAt).toLocaleString()}
            </TableCell> */}
            <TableCell className="w-4">
              <ChevronRight className="h-4 w-4 text-gray-400" />
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}
