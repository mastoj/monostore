const apiBaseUrl = process.env["services__monostore-api__http__0"];

export const getCarts = async () => {
  const url = new URL(`${apiBaseUrl}/carts`);
  const response = await fetch(url);
  const carts = await response.json();
  return carts;
};

export const getCart = async () => {};
