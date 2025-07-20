import { getCarts } from "@/lib/monostore-api/cart-client";
import { Suspense } from "react";
import CartsContent from "./carts-content";
import { connection } from "next/server";

export const experimental_ppr = true;

const Stuff = async () => {
  await connection();

  console.log("Fetching carts...");
  const carts = await getCarts({ operatingChain: "OCSEELG" });

  return (
    <>
      <CartsContent carts={carts} />
    </>
  );
};

export default async function Carts() {
  return (
    <>
      <h1 className="text-2xl font-bold mb-4">Carts Management</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <Stuff />
      </Suspense>
    </>
  );
}
