import { Suspense } from "react";
import Layout from "../../components/layout";
import PurchaseOrdersContent from "./purchase-orders-content";

export default function PurchaseOrders() {
  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">Purchase Orders Management</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <PurchaseOrdersContent />
      </Suspense>
    </Layout>
  );
}
