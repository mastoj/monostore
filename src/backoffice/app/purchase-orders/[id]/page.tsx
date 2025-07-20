import PurchaseOrderDetailsContent from "@/components/purchase-order-details-content";
import {
  getChanges,
  getPurchaseOrder,
} from "@/lib/monostore-api/purchase-order-client";
import { cacheLife } from "next/dist/server/use-cache/cache-life";
import { notFound } from "next/navigation";
import { Suspense } from "react";

export default async function PurchaseOrderDetails({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  "use cache";
  cacheLife("minutes");
  const { id } = await params;
  const orderData = getPurchaseOrder(id);
  const orderEvents = getChanges(id);

  if (!orderData) {
    notFound();
  }

  return (
    <>
      <h1 className="text-2xl font-bold mb-4">Purchase Order Details</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <PurchaseOrderDetailsContent
          purchaseOrderPromise={orderData}
          orderChangesPromise={orderEvents}
        />
      </Suspense>
    </>
  );
}
