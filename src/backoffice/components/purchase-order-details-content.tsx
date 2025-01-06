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
import { Change, PurchaseOrderData } from "@/lib/monostore-api/monostore-api";
import { ChevronRight } from "lucide-react";
import Image from "next/image";
import Link from "next/link";
import { HistoryList } from "./history-list";

interface PurchaseOrderDetailsContentProps {
  purchaseOrder: PurchaseOrderData;
  orderChanges: Change[];
}

export default function PurchaseOrderDetailsContent({
  purchaseOrder,
  orderChanges,
}: PurchaseOrderDetailsContentProps) {
  // const orderChanges = mockPurchaseOrderChanges
  //   .filter(change => change.purchaseOrderId === purchaseOrder.id)
  //   .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())
  const mockOrder = {
    shippingAddress: {
      street: "123 Dummy St",
      city: "Faketown",
      state: "CA",
      zipCode: "12345",
      country: "USA",
    },
    paymentInfo: {
      method: "Credit Card",
      cardLastFour: "1234",
      transactionId: "txn_01ABC23DEFG45HIJ",
    },
  };
  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <CardTitle>Order Information</CardTitle>
        </CardHeader>
        <CardContent>
          <dl className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* <div>
              <dt className="font-semibold">Status:</dt>
              <dd>{purchaseOrder.status}</dd>
            </div> */}
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
              <dd>${purchaseOrder.total.toFixed(2)}</dd>
            </div>
            {/* <div>
              <dt className="font-semibold">Last Updated:</dt>
              <dd>{new Date(purchaseOrder.updatedAt).toLocaleString()}</dd>
            </div> */}
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
              <dd>John Doe</dd>
              {/* <dd>{purchaseOrder.customerName}</dd> */}
            </div>
            <div>
              <dt className="font-semibold">Email:</dt>
              <dd>john@doe.no</dd>
              {/* <dd>{purchaseOrder.customerEmail}</dd> */}
            </div>
            <div>
              <dt className="font-semibold">Phone:</dt>
              <dd>+4712345678</dd>
              {/* <dd>{purchaseOrder.customerPhone}</dd> */}
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
              <dd>{mockOrder.shippingAddress.street}</dd>
            </div>
            <div>
              <dt className="font-semibold">City:</dt>
              <dd>{mockOrder.shippingAddress.city}</dd>
            </div>
            <div>
              <dt className="font-semibold">State:</dt>
              <dd>{mockOrder.shippingAddress.state}</dd>
            </div>
            <div>
              <dt className="font-semibold">Zip Code:</dt>
              <dd>{mockOrder.shippingAddress.zipCode}</dd>
            </div>
            <div>
              <dt className="font-semibold">Country:</dt>
              <dd>{mockOrder.shippingAddress.country}</dd>
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
              <dd>{mockOrder.paymentInfo.method}</dd>
            </div>
            <div>
              <dt className="font-semibold">Card Number:</dt>
              <dd>**** **** **** {mockOrder.paymentInfo.cardLastFour}</dd>
            </div>
            <div>
              <dt className="font-semibold">Transaction ID:</dt>
              <dd>{mockOrder.paymentInfo.transactionId}</dd>
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
          <CardTitle>Order History</CardTitle>
        </CardHeader>
        <CardContent>
          <HistoryList historyItems={orderChanges} />
        </CardContent>
      </Card>
    </div>
  );
}
