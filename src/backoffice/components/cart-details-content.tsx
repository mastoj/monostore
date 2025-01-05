"use client";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { PurchaseOrder } from "@/data/mock-carts";
import { CartData, Change } from "@/lib/monostore-api/monostore-api";
import { ChevronRight } from "lucide-react";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { use } from "react";
import { HistoryList } from "./history-list";

interface CartDetailsContentProps {
  cart: Promise<CartData>;
  cartEvents: Promise<Change[]>;
  purchaseOrders: PurchaseOrder[];
}

export default function CartDetailsContent({
  cart,
  cartEvents,
  purchaseOrders,
}: CartDetailsContentProps) {
  const router = useRouter();
  const resolvedCart = use(cart);
  const resolvedCartEvents = use(cartEvents);

  console.log("==> Resolved cart: ", resolvedCart);

  const handleRowClick = (orderId: string) => {
    router.push(`/purchase-orders/${orderId}`);
  };

  const handleRowMouseEnter = (orderId: string) => {
    router.prefetch(`/purchase-orders/${orderId}`);
  };

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Cart Information</CardTitle>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <dt className="font-semibold">User ID:</dt>
              <dd>{resolvedCart.userId || "N/A"}</dd>
            </div>
            <div>
              <dt className="font-semibold">Session ID:</dt>
              <dd>{resolvedCart.sessionId}</dd>
            </div>
            <div>
              <dt className="font-semibold">Operating chaing:</dt>
              <dd>{resolvedCart.operatingChain}</dd>
            </div>
            {/* <div>
              <dt className="font-semibold">Created At:</dt>
              <dd>{new Date(resolvedCart.createdAt).toLocaleString()}</dd>
            </div>
            <div>
              <dt className="font-semibold">Updated At:</dt>
              <dd>{new Date(resolvedCart.updatedAt).toLocaleString()}</dd>
            </div> */}
          </dl>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Cart Items</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[100px]">Image</TableHead>
                <TableHead>SKU</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Quantity</TableHead>
                <TableHead>Price</TableHead>
                <TableHead>Total</TableHead>
                <TableHead className="w-4"></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {resolvedCart.items.map((item) => (
                <TableRow
                  key={item.product.id}
                  onClick={() => {
                    window.location.href = `https://www.elgiganten.se/p/${item.product.id}`;
                  }}
                  className="cursor-pointer hover:bg-gray-100 transition-colors"
                >
                  <TableCell>
                    <Image
                      src={item.product.primaryImageUrl}
                      alt={item.product.name}
                      width={50}
                      height={50}
                    />
                  </TableCell>
                  <TableCell>{item.product.id}</TableCell>
                  <TableCell>{item.product.name}</TableCell>
                  <TableCell>{item.quantity}</TableCell>
                  <TableCell>${item.product.price.toFixed(2)}</TableCell>
                  <TableCell>
                    ${(item.product.price * item.quantity).toFixed(2)}
                  </TableCell>
                  <TableCell className="w-4">
                    <ChevronRight className="h-4 w-4 text-gray-400" />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Associated Purchase Orders</CardTitle>
        </CardHeader>
        <CardContent>
          {purchaseOrders.length > 0 ? (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Order ID</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Last Updated</TableHead>
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
                      <span>{`${order.id.slice(0, 2)}...${order.id.slice(
                        -4
                      )}`}</span>
                    </TableCell>
                    <TableCell>{order.status}</TableCell>
                    <TableCell>
                      {new Date(order.updatedAt).toLocaleString()}
                    </TableCell>
                    <TableCell className="w-4">
                      <ChevronRight className="h-4 w-4 text-gray-400" />
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          ) : (
            <p>No purchase orders associated with this cart.</p>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Cart History</CardTitle>
        </CardHeader>
        <CardContent>
          <HistoryList historyItems={resolvedCartEvents} />
        </CardContent>
      </Card>
    </div>
  );
}
