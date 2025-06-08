import { getPurchaseOrders } from "@/lib/monostore-api/purchase-order-client";
import { Suspense } from "react";
import Layout from "../../components/layout";
import PurchaseOrdersContent from "./purchase-orders-content";

export const dynamic = "force-dynamic"; // Force dynamic rendering for this page

export default function PurchaseOrders() {
  const purchaseOrdersPromise = getPurchaseOrders({
    operatingChain: "OCSEELG",
    page: 1,
    pageSize: 50,
  });

  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">Purchase Orders Management</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <PurchaseOrdersContent purchaseOrdersPromise={purchaseOrdersPromise} />
      </Suspense>
    </Layout>
  );
}
