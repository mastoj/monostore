"use client";

import { PurchaseOrderList } from "@/components/purchase-order-list";
import { Button } from "@/components/ui/button";
import { PurchaseOrder } from "@/lib/monostore-api/monostore-api";
import {
  GetPurchaseOrdersRequest,
  PaginatedResponse,
  getPurchaseOrders,
} from "@/lib/monostore-api/purchase-order-client";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { use, useCallback, useState } from "react";
import { PurchaseOrderFilter } from "../../components/purchase-order-filter";

export type PurchaseOrdersContentProps = {
  purchaseOrdersPromise: Promise<PaginatedResponse<PurchaseOrder>>;
};

export default function PurchaseOrdersContent({
  purchaseOrdersPromise,
}: PurchaseOrdersContentProps) {
  const initialPaginatedResponse = use(purchaseOrdersPromise);
  const [paginatedResponse, setPaginatedResponse] = useState<
    PaginatedResponse<PurchaseOrder>
  >(initialPaginatedResponse);
  const [isLoading, setIsLoading] = useState(false);
  const [isExpanded, setIsExpanded] = useState(false);
  const [filters, setFilters] = useState({
    status: "",
    orderId: "",
    cartId: "",
    customerName: "",
  });

  const handleFilterChange = (key: string, value: string) => {
    setFilters((prev) => ({ ...prev, [key]: value }));
  };

  const handleFilterSubmit = () => {
    // Reset to page 1 when filters are applied
    fetchPage(1, paginatedResponse.pagination.pageSize);
  };

  const fetchPage = useCallback(
    async (page: number, pageSize: number) => {
      try {
        setIsLoading(true);
        const requestParams: GetPurchaseOrdersRequest = {
          operatingChain: "OCSEELG",
          page,
          pageSize,
        };

        // Add any active filters
        if (filters.cartId) {
          requestParams.cartId = filters.cartId;
        }

        const result = await getPurchaseOrders(requestParams);
        setPaginatedResponse(result);
      } catch (error) {
        console.error("Error fetching purchase orders:", error);
      } finally {
        setIsLoading(false);
      }
    },
    [filters]
  );

  const handlePageChange = (newPage: number) => {
    fetchPage(newPage, paginatedResponse.pagination.pageSize);
  };

  return (
    <div className="space-y-4">
      <PurchaseOrderFilter
        filters={filters}
        onFilterChange={handleFilterChange}
        onFilterSubmit={handleFilterSubmit}
        isExpanded={isExpanded}
        onToggleExpand={() => setIsExpanded(!isExpanded)}
      />

      {isLoading ? (
        <div className="flex justify-center py-8">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
        </div>
      ) : (
        <>
          <PurchaseOrderList purchaseOrders={paginatedResponse.data} />

          {/* Pagination Controls */}
          <div className="flex items-center justify-between mt-4">
            <div className="text-sm text-gray-500">
              Showing{" "}
              {(paginatedResponse.pagination.currentPage - 1) *
                paginatedResponse.pagination.pageSize +
                1}{" "}
              to{" "}
              {Math.min(
                paginatedResponse.pagination.currentPage *
                  paginatedResponse.pagination.pageSize,
                paginatedResponse.pagination.totalItems
              )}{" "}
              of {paginatedResponse.pagination.totalItems} orders
            </div>
            <div className="flex space-x-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() =>
                  handlePageChange(paginatedResponse.pagination.currentPage - 1)
                }
                disabled={paginatedResponse.pagination.currentPage <= 1}
              >
                <ChevronLeft className="h-4 w-4" />
                <span className="sr-only">Previous Page</span>
              </Button>
              <div className="flex items-center space-x-1">
                {Array.from(
                  {
                    length: Math.min(
                      5,
                      paginatedResponse.pagination.totalPages
                    ),
                  },
                  (_, i) => {
                    const pageNum = i + 1;
                    const isCurrentPage =
                      pageNum === paginatedResponse.pagination.currentPage;
                    return (
                      <Button
                        key={pageNum}
                        variant={isCurrentPage ? "default" : "outline"}
                        size="sm"
                        onClick={() => handlePageChange(pageNum)}
                        disabled={isCurrentPage}
                        className="w-8 h-8"
                      >
                        {pageNum}
                      </Button>
                    );
                  }
                )}
                {paginatedResponse.pagination.totalPages > 5 && (
                  <>
                    <span className="px-1">...</span>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() =>
                        handlePageChange(
                          paginatedResponse.pagination.totalPages
                        )
                      }
                      className="w-8 h-8"
                    >
                      {paginatedResponse.pagination.totalPages}
                    </Button>
                  </>
                )}
              </div>
              <Button
                variant="outline"
                size="sm"
                onClick={() =>
                  handlePageChange(paginatedResponse.pagination.currentPage + 1)
                }
                disabled={
                  paginatedResponse.pagination.currentPage >=
                  paginatedResponse.pagination.totalPages
                }
              >
                <ChevronRight className="h-4 w-4" />
                <span className="sr-only">Next Page</span>
              </Button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
