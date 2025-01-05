'use client'

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { ChevronDown, ChevronUp } from 'lucide-react'

interface PurchaseOrderFilterProps {
  filters: {
    status: string;
    orderId: string;
    cartId: string;
    customerName: string;
  };
  onFilterChange: (key: string, value: string) => void;
  onFilterSubmit: () => void;
  isExpanded: boolean;
  onToggleExpand: () => void;
}

export function PurchaseOrderFilter({ filters, onFilterChange, onFilterSubmit, isExpanded, onToggleExpand }: PurchaseOrderFilterProps) {
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onFilterSubmit()
  }

  return (
    <div className="bg-white shadow-md rounded-lg p-4 mb-4 overflow-hidden transition-all duration-300 ease-in-out">
      <div className="flex justify-between items-center mb-2">
        <h2 className="text-lg font-semibold">Filters</h2>
        <Button
          variant="ghost"
          size="sm"
          onClick={onToggleExpand}
        >
          {isExpanded ? (
            <>
              <ChevronUp className="h-4 w-4 mr-2" />
              Hide Filters
            </>
          ) : (
            <>
              <ChevronDown className="h-4 w-4 mr-2" />
              Show Filters
            </>
          )}
        </Button>
      </div>
      <div className={`transition-all duration-300 ease-in-out ${isExpanded ? 'max-h-[1000px] opacity-100' : 'max-h-0 opacity-0'}`}>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label htmlFor="status">Status</Label>
              <Select value={filters.status} onValueChange={(value) => onFilterChange('status', value)}>
                <SelectTrigger id="status">
                  <SelectValue placeholder="Select status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Statuses</SelectItem>
                  <SelectItem value="Pending">Pending</SelectItem>
                  <SelectItem value="Processing">Processing</SelectItem>
                  <SelectItem value="Shipped">Shipped</SelectItem>
                  <SelectItem value="Delivered">Delivered</SelectItem>
                  <SelectItem value="Cancelled">Cancelled</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="orderId">Order ID</Label>
              <Input 
                id="orderId" 
                value={filters.orderId} 
                onChange={(e) => onFilterChange('orderId', e.target.value)} 
                placeholder="Enter Order ID" 
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="cartId">Cart ID</Label>
              <Input 
                id="cartId" 
                value={filters.cartId} 
                onChange={(e) => onFilterChange('cartId', e.target.value)} 
                placeholder="Enter Cart ID" 
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="customerName">Customer Name</Label>
              <Input 
                id="customerName" 
                value={filters.customerName} 
                onChange={(e) => onFilterChange('customerName', e.target.value)} 
                placeholder="Enter Customer Name" 
              />
            </div>
          </div>
          <Button type="submit">Apply Filters</Button>
        </form>
      </div>
    </div>
  )
}

