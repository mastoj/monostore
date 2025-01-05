import { v4 as uuid } from 'uuid';

export interface CartItem {
  sku: string;
  quantity: number;
  price: number;
  name: string;
  image: string;
}

export interface Cart {
  id: string;
  shortId: string;
  userId: string | null;
  sessionId: string;
  country: 'Sweden' | 'Finland' | 'Norway' | 'Denmark';
  items: CartItem[];
  createdAt: string;
  updatedAt: string;
}

export interface CartEvent {
  id: string;
  cartId: string;
  event: string;
  data: any;
  timestamp: string;
}

export const mockCarts: Cart[] = [
  {
    id: "123e4567-e89b-12d3-a456-426614174000",
    shortId: 'CART001',
    userId: 'USER123',
    sessionId: 'SESSION001',
    country: 'Sweden',
    items: [
      { sku: 'SKU001', quantity: 2, price: 29.99, name: 'Product 1', image: '/placeholder.svg?height=50&width=50' },
      { sku: 'SKU002', quantity: 1, price: 49.99, name: 'Product 2', image: '/placeholder.svg?height=50&width=50' },
    ],
    createdAt: '2023-05-01T10:00:00Z',
    updatedAt: '2023-05-01T10:15:00Z',
  },
  {
    id: "223e4567-e89b-12d3-a456-426614174001",
    shortId: 'CART002',
    userId: null,
    sessionId: 'SESSION002',
    country: 'Finland',
    items: [
      { sku: 'SKU003', quantity: 3, price: 19.99, name: 'Product 3', image: '/placeholder.svg?height=50&width=50' },
    ],
    createdAt: '2023-05-02T14:30:00Z',
    updatedAt: '2023-05-02T14:45:00Z',
  },
  {
    id: uuid(),
    shortId: 'CART003',
    userId: 'USER456',
    sessionId: 'SESSION003',
    country: 'Norway',
    items: [
      { sku: 'SKU001', quantity: 1, price: 29.99, name: 'Product 1', image: '/placeholder.svg?height=50&width=50' },
      { sku: 'SKU004', quantity: 2, price: 39.99, name: 'Product 4', image: '/placeholder.svg?height=50&width=50' },
    ],
    createdAt: '2023-05-03T09:15:00Z',
    updatedAt: '2023-05-03T09:30:00Z',
  },
  {
    id: uuid(),
    shortId: 'CART004',
    userId: 'USER789',
    sessionId: 'SESSION004',
    country: 'Denmark',
    items: [
      { sku: 'SKU002', quantity: 1, price: 49.99, name: 'Product 2', image: '/placeholder.svg?height=50&width=50' },
      { sku: 'SKU005', quantity: 4, price: 14.99, name: 'Product 5', image: '/placeholder.svg?height=50&width=50' },
    ],
    createdAt: '2023-05-04T16:45:00Z',
    updatedAt: '2023-05-04T17:00:00Z',
  },
  {
    id: uuid(),
    shortId: 'CART005',
    userId: null,
    sessionId: 'SESSION005',
    country: 'Sweden',
    items: [
      { sku: 'SKU003', quantity: 2, price: 19.99, name: 'Product 3', image: '/placeholder.svg?height=50&width=50' },
      { sku: 'SKU004', quantity: 1, price: 39.99, name: 'Product 4', image: '/placeholder.svg?height=50&width=50' },
    ],
    createdAt: '2023-05-05T11:30:00Z',
    updatedAt: '2023-05-05T11:45:00Z',
  },
];

export const mockCartEvents: CartEvent[] = [
  { 
    id: uuid(),
    cartId: "123e4567-e89b-12d3-a456-426614174000",
    event: "CartCreated",
    data: { userId: 'USER123', sessionId: 'SESSION001', country: 'Sweden' },
    timestamp: '2023-05-01T10:00:00Z'
  },
  { 
    id: uuid(),
    cartId: "123e4567-e89b-12d3-a456-426614174000",
    event: "ItemAdded",
    data: { sku: 'SKU001', quantity: 1, price: 29.99, name: 'Product 1' },
    timestamp: '2023-05-01T10:05:00Z'
  },
  { 
    id: uuid(),
    cartId: "123e4567-e89b-12d3-a456-426614174000",
    event: "ItemAdded",
    data: { sku: 'SKU002', quantity: 1, price: 49.99, name: 'Product 2' },
    timestamp: '2023-05-01T10:10:00Z'
  },
  { 
    id: uuid(),
    cartId: "123e4567-e89b-12d3-a456-426614174000",
    event: "ItemQuantityUpdated",
    data: { sku: 'SKU001', oldQuantity: 1, newQuantity: 2 },
    timestamp: '2023-05-01T10:15:00Z'
  },
  { 
    id: uuid(),
    cartId: "223e4567-e89b-12d3-a456-426614174001",
    event: "CartCreated",
    data: { sessionId: 'SESSION002', country: 'Finland' },
    timestamp: '2023-05-02T14:30:00Z'
  },
  { 
    id: uuid(),
    cartId: "223e4567-e89b-12d3-a456-426614174001",
    event: "ItemAdded",
    data: { sku: 'SKU003', quantity: 3, price: 19.99, name: 'Product 3' },
    timestamp: '2023-05-02T14:35:00Z'
  },
  { id: uuid(), cartId: "123e4567-e89b-12d3-a456-426614174000", event: 'ItemRemoved', data: {sku: 'SKU001', quantity: 1}, timestamp: '2023-05-01T10:20:00Z' },
  { id: uuid(), cartId: "223e4567-e89b-12d3-a456-426614174001", event: 'ItemRemoved', data: {sku: 'SKU003', quantity: 1}, timestamp: '2023-05-02T14:40:00Z' },
  { id: uuid(), cartId: "123e4567-e89b-12d3-a456-426614174000", event: 'CartAbandoned', data: {}, timestamp: '2023-05-01T10:25:00Z' },
  { id: uuid(), cartId: "223e4567-e89b-12d3-a456-426614174001", event: 'CartCheckedOut', data: {}, timestamp: '2023-05-02T14:50:00Z' },

];


export interface PurchaseOrder {
  id: string;
  cartId: string;
  status: 'Pending' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled';
  updatedAt: string;
  totalAmount: number;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  shippingAddress: {
    street: string;
    city: string;
    state: string;
    zipCode: string;
    country: string;
  };
  paymentInfo: {
    method: string;
    cardLastFour: string;
    transactionId: string;
  };
  items: {
    sku: string;
    name: string;
    quantity: number;
    price: number;
  }[];
}

export const mockPurchaseOrders: PurchaseOrder[] = [
  {
    id: "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    cartId: "123e4567-e89b-12d3-a456-426614174000",
    status: 'Processing',
    updatedAt: '2023-05-01T11:30:00Z',
    totalAmount: 109.97,
    customerName: 'John Doe',
    customerEmail: 'john.doe@example.com',
    customerPhone: '+1 (555) 123-4567',
    shippingAddress: {
      street: '123 Main St',
      city: 'Anytown',
      state: 'CA',
      zipCode: '12345',
      country: 'USA',
    },
    paymentInfo: {
      method: 'Credit Card',
      cardLastFour: '1234',
      transactionId: 'txn_1234567890',
    },
    items: [
      { sku: 'SKU001', name: 'Product 1', quantity: 2, price: 29.99 },
      { sku: 'SKU002', name: 'Product 2', quantity: 1, price: 49.99 },
    ],
  },
  {
    id: "7f6c64e1-e3b1-4e6b-b5a9-8a2b3e6f4d5c",
    cartId: "223e4567-e89b-12d3-a456-426614174001",
    status: 'Shipped',
    updatedAt: '2023-05-02T15:30:00Z',
    totalAmount: 59.97,
    customerName: 'Jane Smith',
    customerEmail: 'jane.smith@example.com',
    customerPhone: '+1 (555) 987-6543',
    shippingAddress: {
      street: '456 Elm St',
      city: 'Other City',
      state: 'NY',
      zipCode: '67890',
      country: 'USA',
    },
    paymentInfo: {
      method: 'PayPal',
      cardLastFour: 'N/A',
      transactionId: 'pp_9876543210',
    },
    items: [
      { sku: 'SKU003', name: 'Product 3', quantity: 3, price: 19.99 },
    ],
  },
  {
    id: "3a4b5c6d-7e8f-9g0h-1i2j-3k4l5m6n7o8p",
    cartId: "323e4567-e89b-12d3-a456-426614174002",
    status: 'Delivered',
    updatedAt: '2023-05-03T09:45:00Z',
    totalAmount: 79.98,
    customerName: 'Alice Johnson',
    customerEmail: 'alice.johnson@example.com',
    customerPhone: '+1 (555) 246-8135',
    shippingAddress: {
      street: '789 Oak St',
      city: 'Another Town',
      state: 'TX',
      zipCode: '54321',
      country: 'USA',
    },
    paymentInfo: {
      method: 'Credit Card',
      cardLastFour: '5678',
      transactionId: 'txn_2468101214',
    },
    items: [
      { sku: 'SKU004', name: 'Product 4', quantity: 2, price: 39.99 },
    ],
  },
];


export interface PurchaseOrderChange {
  id: string;
  purchaseOrderId: string;
  event: string;
  data: any;
  timestamp: string;
}

export const mockPurchaseOrderChanges: PurchaseOrderChange[] = [
  {
    id: uuid(),
    purchaseOrderId: "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    event: "OrderCreated",
    data: { status: 'Pending', totalAmount: 109.97 },
    timestamp: '2023-05-01T11:00:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    event: "StatusUpdated",
    data: { oldStatus: 'Pending', newStatus: 'Processing' },
    timestamp: '2023-05-01T11:30:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "7f6c64e1-e3b1-4e6b-b5a9-8a2b3e6f4d5c",
    event: "OrderCreated",
    data: { status: 'Pending', totalAmount: 59.97 },
    timestamp: '2023-05-02T15:00:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "7f6c64e1-e3b1-4e6b-b5a9-8a2b3e6f4d5c",
    event: "StatusUpdated",
    data: { oldStatus: 'Pending', newStatus: 'Processing' },
    timestamp: '2023-05-02T15:15:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "7f6c64e1-e3b1-4e6b-b5a9-8a2b3e6f4d5c",
    event: "StatusUpdated",
    data: { oldStatus: 'Processing', newStatus: 'Shipped' },
    timestamp: '2023-05-02T15:30:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "3a4b5c6d-7e8f-9g0h-1i2j-3k4l5m6n7o8p",
    event: "OrderCreated",
    data: { status: 'Pending', totalAmount: 79.98 },
    timestamp: '2023-05-03T09:00:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "3a4b5c6d-7e8f-9g0h-1i2j-3k4l5m6n7o8p",
    event: "StatusUpdated",
    data: { oldStatus: 'Pending', newStatus: 'Processing' },
    timestamp: '2023-05-03T09:15:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "3a4b5c6d-7e8f-9g0h-1i2j-3k4l5m6n7o8p",
    event: "StatusUpdated",
    data: { oldStatus: 'Processing', newStatus: 'Shipped' },
    timestamp: '2023-05-03T09:30:00Z'
  },
  {
    id: uuid(),
    purchaseOrderId: "3a4b5c6d-7e8f-9g0h-1i2j-3k4l5m6n7o8p",
    event: "StatusUpdated",
    data: { oldStatus: 'Shipped', newStatus: 'Delivered' },
    timestamp: '2023-05-03T09:45:00Z'
  },
];

