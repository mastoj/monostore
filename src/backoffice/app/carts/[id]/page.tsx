import CartDetailsContent from "@/components/cart-details-content";
import { getCart, getChanges } from "@/lib/monostore-api/cart-client";
import { getPurchaseOrders } from "@/lib/monostore-api/purchase-order-client";
import { notFound } from "next/navigation";
import { Suspense } from "react";
import Layout from "../../../components/layout";
import { cacheLife } from "next/dist/server/use-cache/cache-life";
import { cacheTag } from "next/dist/server/use-cache/cache-tag";

export default async function CartDetails({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  "use cache";
  cacheLife("minutes");
  const { id } = await params;
  cacheTag(`cart-${id}`);
  const cartData = await getCart(id);
  const cartEvents = await getChanges(id);
  const purchaseOrders = await getPurchaseOrders({
    operatingChain: "OCSEELG",
    cartId: id,
  });
  // const purchaseOrders: PurchaseOrder[] = [];
  // mockPurchaseOrders.filter(
  //   (po) => po.cartId === params.id
  // );

  if (!cartData) {
    notFound();
  }

  return (
    <>
      <h1 className="text-2xl font-bold mb-4">Cart Details: {id}</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <CartDetailsContent
          cart={cartData}
          cartEvents={cartEvents}
          purchaseOrders={purchaseOrders}
        />
      </Suspense>
    </>
  );
}
