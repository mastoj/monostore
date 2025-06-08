import { getCarts } from "@/lib/monostore-api/cart-client";
import { Suspense } from "react";
import Layout from "../../components/layout";
import CartsContent from "./carts-content";

export const dynamic = "force-dynamic"; // Force dynamic rendering for this page

export default function Carts() {
  const carts = getCarts({ operatingChain: "OCSEELG" });
  return (
    <Layout>
      <h1 className="text-2xl font-bold mb-4">Carts Management</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <CartsContent carts={carts} />
      </Suspense>
    </Layout>
  );
}
