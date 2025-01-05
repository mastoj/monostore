import CartDetailsContent from "@/components/cart-details-content";
import { notFound } from "next/navigation";
import { Suspense } from "react";
import Layout from "../../components/layout";
import {
  mockCartEvents,
  mockCarts,
  mockPurchaseOrders,
} from "../../data/mock-carts";

export default function CartDetails({ params }: { params: { id: string } }) {
  const cart = mockCarts.find((c) => c.id === params.id);
  const cartEvents = mockCartEvents.filter((e) => e.cartId === params.id);
  const purchaseOrders = mockPurchaseOrders.filter(
    (po) => po.cartId === params.id
  );

  if (!cart) {
    notFound();
  }

  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">Cart Details: {cart.shortId}</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <CartDetailsContent
          cart={cart}
          cartEvents={cartEvents}
          purchaseOrders={purchaseOrders}
        />
      </Suspense>
    </Layout>
  );
}
