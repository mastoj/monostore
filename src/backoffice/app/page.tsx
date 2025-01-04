"use client";

import { useActionState } from "react";
import { createCart } from "./cart-actions";

export default function Home() {
  const [state, formAction, pending] = useActionState(
    createCart.bind(null, "123"),
    {}
  );

  return (
    <div>
      Hello
      <form action={formAction}>
        <button
          type="submit"
          value="Create Cart"
          disabled={pending}
          className="px-4 py-2 rounded bg-green-600"
        >
          {/* Spinner */}
          {pending ? <span>Loading...</span> : "Create Cart"}
        </button>
      </form>
      <pre>{JSON.stringify(state, null, 2)}</pre>
    </div>
  );
}
