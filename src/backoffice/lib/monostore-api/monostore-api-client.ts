import {
  Cart,
  Change,
  GrainResultOfCartDataAndCartError,
} from "./monostore-api";

const apiBaseUrl = process.env["services__monostore-api__http__0"];

export type GetCartsRequest = {
  operatingChain: string;
};
export const getCarts = async ({ operatingChain }: GetCartsRequest) => {
  const url = new URL(`${apiBaseUrl}/cart`);
  url.searchParams.append("operatingChain", operatingChain);
  const response = await fetch(url);
  const carts = await response.json();
  return carts satisfies Cart[];
};

export const getCart = async (id: string) => {
  const url = new URL(`${apiBaseUrl}/cart/${id}`);
  const response = await fetch(url);
  const result =
    (await response.json()) satisfies GrainResultOfCartDataAndCartError;
  if (result.error) {
    throw new Error(result.error);
  }
  return result.data satisfies Cart[];
};

export const getChanges = async (id: string) => {
  const url = new URL(`${apiBaseUrl}/cart/${id}/changes`);
  const response = await fetch(url);
  const changes = await response.json();
  return changes satisfies Change[];
};
