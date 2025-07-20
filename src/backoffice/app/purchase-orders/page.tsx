import { getPurchaseOrders } from "@/lib/monostore-api/purchase-order-client";
import PurchaseOrdersContent from "./purchase-orders-content";
import { Suspense } from "react";
import { connection } from "next/server";
import { cacheLife } from "next/dist/server/use-cache/cache-life";

export const experimental_ppr = true;

const PurchaseOrdersLoader = async () => {
  const purchaseOrders = await getPurchaseOrders({
    operatingChain: "OCSEELG",
    page: 1,
    pageSize: 50,
  });

  return (
    <>
      <PurchaseOrdersContent purchaseOrders={purchaseOrders} />
    </>
  );
};

export default async function PurchaseOrders() {
  "use cache";
  cacheLife("minutes");
  return (
    <>
      <h1 className="text-2xl font-bold mb-4">Purchase Orders Management</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <PurchaseOrdersLoader />
      </Suspense>
    </>
  );
}
