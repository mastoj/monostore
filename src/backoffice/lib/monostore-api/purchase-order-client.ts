import {
  Change,
  GrainResultOfPurchaseOrderDataAndCheckoutError,
  PurchaseOrder,
} from "./monostore-api";

const apiBaseUrl = `${process.env["services__monostore-api__http__0"]}/checkout`;

export type getPurchaseOrderRequest = {
  operatingChain: string;
  cartId?: string;
};
export const getPurchaseOrders = async ({
  operatingChain,
  cartId,
}: getPurchaseOrderRequest) => {
  const url = new URL(apiBaseUrl);
  url.searchParams.append("operatingChain", operatingChain);
  if (cartId) {
    url.searchParams.append("cartId", cartId);
  }
  const response = await fetch(url);
  const purchaseOrders = await response.json();
  return purchaseOrders satisfies PurchaseOrder[];
};

export const getPurchaseOrder = async (id: string) => {
  const url = new URL(`${apiBaseUrl}/${id}`);
  const response = await fetch(url);
  const result =
    (await response.json()) satisfies GrainResultOfPurchaseOrderDataAndCheckoutError;
  if (result.error) {
    throw new Error(result.error);
  }
  return result.data satisfies PurchaseOrder[];
};

export const getChanges = async (id: string) => {
  const url = new URL(`${apiBaseUrl}/${id}/changes`);
  const response = await fetch(url);
  const changes = await response.json();
  return changes satisfies Change[];
};
