import PurchaseOrderDetailsContent from "@/components/purchase-order-details-content";
import { notFound } from "next/navigation";
import { Suspense } from "react";
import Layout from "../../components/layout";
import {
  mockPurchaseOrderChanges,
  mockPurchaseOrders,
} from "../../data/mock-carts";

export default function PurchaseOrderDetails({
  params,
}: {
  params: { id: string };
}) {
  const order = mockPurchaseOrders.find((o) => o.id === params.id);

  if (!order) {
    notFound();
  }

  const orderChanges = mockPurchaseOrderChanges.filter(
    (change) => change.purchaseOrderId === params.id
  );

  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">
        Purchase Order Details: {order.id}
      </h1>
      <Suspense fallback={<div>Loading...</div>}>
        <PurchaseOrderDetailsContent
          purchaseOrder={order}
          orderChanges={orderChanges}
        />
      </Suspense>
    </Layout>
  );
}
