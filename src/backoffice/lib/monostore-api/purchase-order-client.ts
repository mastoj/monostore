"use server";
import {
  Change,
  GrainResultOfPurchaseOrderDataAndCheckoutError,
  PurchaseOrder,
  PurchaseOrderData,
} from "./monostore-api";

const apiBaseUrl = `${process.env["services__monostore-api__http__0"]}/checkout`;

export type PaginatedResponse<T> = {
  data: T[];
  pagination: {
    currentPage: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
  };
};

export type GetPurchaseOrdersRequest = {
  operatingChain: string;
  cartId?: string;
  page?: number;
  pageSize?: number;
};

export const getPurchaseOrders = async ({
  operatingChain,
  cartId,
  page = 1,
  pageSize = 50,
}: GetPurchaseOrdersRequest): Promise<PaginatedResponse<PurchaseOrder>> => {
  console.log("==> Base url: ", apiBaseUrl);
  const url = new URL(apiBaseUrl);
  url.searchParams.append("operatingChain", operatingChain);
  if (cartId) {
    url.searchParams.append("cartId", cartId);
  }
  url.searchParams.append("page", page.toString());
  url.searchParams.append("pageSize", pageSize.toString());

  const response = await fetch(url);
  const result = await response.json();
  return result as PaginatedResponse<PurchaseOrder>;
};

export const getPurchaseOrder = async (
  id: string
): Promise<PurchaseOrderData> => {
  const url = new URL(`${apiBaseUrl}/${id}`);
  const response = await fetch(url);
  const result =
    (await response.json()) satisfies GrainResultOfPurchaseOrderDataAndCheckoutError;
  if (result.error) {
    throw new Error(result.error);
  }
  return result.data satisfies PurchaseOrderData;
};

export const getChanges = async (id: string) => {
  const url = new URL(`${apiBaseUrl}/${id}/changes`);
  const response = await fetch(url);
  const changes = await response.json();
  return changes satisfies Change[];
};
