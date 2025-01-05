"use client";

import { PurchaseOrder, PurchaseOrderChange } from "@/app/data/mock-carts";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { HistoryList } from "./history-list";

interface PurchaseOrderDetailsContentProps {
  purchaseOrder: PurchaseOrder;
  orderChanges: PurchaseOrderChange[];
}

export default function PurchaseOrderDetailsContent({
  purchaseOrder,
  orderChanges,
}: PurchaseOrderDetailsContentProps) {
  const router = useRouter();

  // const orderChanges = mockPurchaseOrderChanges
  //   .filter(change => change.purchaseOrderId === purchaseOrder.id)
  //   .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Order Information</CardTitle>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <dt className="font-semibold">Status:</dt>
              <dd>{purchaseOrder.status}</dd>
            </div>
            <div>
              <dt className="font-semibold">Cart ID:</dt>
              <dd className="flex items-center space-x-2">
                <span>{`${purchaseOrder.cartId.slice(
                  0,
                  2
                )}...${purchaseOrder.cartId.slice(-4)}`}</span>
                <Link
                  href={`/carts/${purchaseOrder.cartId}`}
                  className="p-0 h-auto font-normal underline text-blue-600 hover:text-blue-800 hover:no-underline"
                >
                  View Cart
                </Link>
              </dd>
            </div>
            <div>
              <dt className="font-semibold">Total Amount:</dt>
              <dd>${purchaseOrder.totalAmount.toFixed(2)}</dd>
            </div>
            <div>
              <dt className="font-semibold">Last Updated:</dt>
              <dd>{new Date(purchaseOrder.updatedAt).toLocaleString()}</dd>
            </div>
          </dl>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Customer Information</CardTitle>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <dt className="font-semibold">Name:</dt>
              <dd>{purchaseOrder.customerName}</dd>
            </div>
            <div>
              <dt className="font-semibold">Email:</dt>
              <dd>{purchaseOrder.customerEmail}</dd>
            </div>
            <div>
              <dt className="font-semibold">Phone:</dt>
              <dd>{purchaseOrder.customerPhone}</dd>
            </div>
          </dl>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Shipping Information</CardTitle>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <dt className="font-semibold">Address:</dt>
              <dd>{purchaseOrder.shippingAddress.street}</dd>
            </div>
            <div>
              <dt className="font-semibold">City:</dt>
              <dd>{purchaseOrder.shippingAddress.city}</dd>
            </div>
            <div>
              <dt className="font-semibold">State:</dt>
              <dd>{purchaseOrder.shippingAddress.state}</dd>
            </div>
            <div>
              <dt className="font-semibold">Zip Code:</dt>
              <dd>{purchaseOrder.shippingAddress.zipCode}</dd>
            </div>
            <div>
              <dt className="font-semibold">Country:</dt>
              <dd>{purchaseOrder.shippingAddress.country}</dd>
            </div>
          </dl>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Payment Information</CardTitle>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <dt className="font-semibold">Payment Method:</dt>
              <dd>{purchaseOrder.paymentInfo.method}</dd>
            </div>
            <div>
              <dt className="font-semibold">Card Number:</dt>
              <dd>**** **** **** {purchaseOrder.paymentInfo.cardLastFour}</dd>
            </div>
            <div>
              <dt className="font-semibold">Transaction ID:</dt>
              <dd>{purchaseOrder.paymentInfo.transactionId}</dd>
            </div>
          </dl>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Order Items</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>SKU</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Quantity</TableHead>
                <TableHead>Price</TableHead>
                <TableHead>Total</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {purchaseOrder.items.map((item) => (
                <TableRow key={item.sku}>
                  <TableCell>{item.sku}</TableCell>
                  <TableCell>{item.name}</TableCell>
                  <TableCell>{item.quantity}</TableCell>
                  <TableCell>${item.price.toFixed(2)}</TableCell>
                  <TableCell>
                    ${(item.price * item.quantity).toFixed(2)}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Order History</CardTitle>
        </CardHeader>
        <CardContent>
          <HistoryList historyItems={orderChanges} />
        </CardContent>
      </Card>
    </div>
  );
}
