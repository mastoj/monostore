import CartDetailsContent from "@/components/cart-details-content";
import { PurchaseOrder } from "@/data/mock-carts";
import { getCart, getChanges } from "@/lib/monostore-api/monostore-api-client";
import { notFound } from "next/navigation";
import { Suspense } from "react";
import Layout from "../../../components/layout";

export default async function CartDetails({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const cartData = getCart(id);
  const cartEvents = getChanges(id);
  const purchaseOrders: PurchaseOrder[] = [];
  // mockPurchaseOrders.filter(
  //   (po) => po.cartId === params.id
  // );

  if (!cartData) {
    notFound();
  }

  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">Cart Details: {id}</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <CartDetailsContent
          cart={cartData}
          cartEvents={cartEvents}
          purchaseOrders={purchaseOrders}
        />
      </Suspense>
    </Layout>
  );
}
