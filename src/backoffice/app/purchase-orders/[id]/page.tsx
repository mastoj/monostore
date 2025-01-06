import PurchaseOrderDetailsContent from "@/components/purchase-order-details-content";
import {
  getChanges,
  getPurchaseOrder,
} from "@/lib/monostore-api/purchase-order-client";
import { notFound } from "next/navigation";
import { Suspense } from "react";
import Layout from "../../../components/layout";

export default async function PurchaseOrderDetails({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const orderData = await getPurchaseOrder(id);
  const orderEvents = await getChanges(id);

  if (!orderData) {
    notFound();
  }

  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">
        Purchase Order Details: {orderData.id}
      </h1>
      <Suspense fallback={<div>Loading...</div>}>
        <PurchaseOrderDetailsContent
          purchaseOrder={orderData}
          orderChanges={orderEvents}
        />
      </Suspense>
    </Layout>
  );
}
