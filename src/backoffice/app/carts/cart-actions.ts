"use server";

const cartBaseUrl = process.env["services__monostore-api__http__0"];
export const createCart = async (sessionId: string) => {
  const url = new URL(`${cartBaseUrl}/cart`);
  console.log("==> Creating cart with session ID", sessionId, url);
  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Cookie: `session-id=${sessionId}`,
    },
    body: JSON.stringify({}),
  });
  console.log("==> Status: ", response.status);
  const cart = await response.json();
  console.log("==> Cart created", cart);
  return cart;
};
