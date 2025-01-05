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

interface CartFilterProps {
  filters: {
    country: string;
    cartId: string;
    sku: string;
    userId: string;
    sessionId: string;
  };
  onFilterChange: (key: string, value: string) => void;
  onFilterSubmit: () => void;
  isExpanded: boolean;
  onToggleExpand: () => void;
}

export function CartFilter({ filters, onFilterChange, onFilterSubmit, isExpanded, onToggleExpand }: CartFilterProps) {
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
                <Label htmlFor="country">Country</Label>
                <Select value={filters.country} onValueChange={(value) => onFilterChange('country', value)}>
                  <SelectTrigger id="country">
                    <SelectValue placeholder="Select country" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">All Countries</SelectItem>
                    <SelectItem value="Sweden">Sweden</SelectItem>
                    <SelectItem value="Finland">Finland</SelectItem>
                    <SelectItem value="Norway">Norway</SelectItem>
                    <SelectItem value="Denmark">Denmark</SelectItem>
                  </SelectContent>
                </Select>
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
                <Label htmlFor="sku">SKU</Label>
                <Input 
                  id="sku" 
                  value={filters.sku} 
                  onChange={(e) => onFilterChange('sku', e.target.value)} 
                  placeholder="Enter SKU" 
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="userId">User ID</Label>
                <Input 
                  id="userId" 
                  value={filters.userId} 
                  onChange={(e) => onFilterChange('userId', e.target.value)} 
                  placeholder="Enter User ID" 
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="sessionId">Session ID</Label>
                <Input 
                  id="sessionId" 
                  value={filters.sessionId} 
                  onChange={(e) => onFilterChange('sessionId', e.target.value)} 
                  placeholder="Enter Session ID" 
                />
              </div>
            </div>
            <Button type="submit">Apply Filters</Button>
          </form>
        </div>
    </div>
  )
}

