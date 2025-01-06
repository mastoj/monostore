/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface AddItemRequest {
  operatingChain: string;
  productId: string;
}

export type AItemDto = {
  aArticleNumber?: string;
  /** @format double */
  aPrice?: number | null;
  /** @format double */
  aPriceExVat?: number | null;
  aTitle?: string;
};

export interface AttributeDto {
  name?: string;
  purpose?: string;
  /** @format int32 */
  sequence?: number;
  value?: string;
  structure?: string;
  identifier?: string;
  description?: string;
  presetValueIdentifier?: string;
  dataType?: string;
}

export type BGrade = {
  bGradeId?: string;
  bGradeTitle?: string;
} | null;

export interface Brand {
  brandId?: string;
  brandName?: string;
}

export interface Cart {
  /** @format uuid */
  id: string;
  operatingChain: string;
  status: CartStatus;
  items: CartItem[];
  sessionId: string;
  userId: string | null;
  /** @format double */
  total: number;
  /**
   * @format double
   * @default 0
   */
  totalExVat?: number;
  /**
   * @format double
   * @default 0
   */
  beforePriceTotal?: number;
  /**
   * @format double
   * @default 0
   */
  beforePriceExVatTotal?: number;
  /** @default "" */
  currency?: string;
  /**
   * @format int32
   * @default 1
   */
  version?: number;
}

export type CartData = {
  /** @format uuid */
  id: string;
  /** @format int32 */
  version: number;
  operatingChain: string;
  status: CartStatus;
  items: CartItem[];
  /** @format double */
  total: number;
  /** @format double */
  totalExVat: number;
  /** @format double */
  beforePriceTotal: number;
  /** @format double */
  beforePriceExVatTotal: number;
  sessionId: string;
  userId: string | null;
};

export type CartError = {
  message?: string;
  type?: CartErrorType;
} | null;

export type CartErrorType = number;

export interface CartItem {
  product: Product;
  /** @format int32 */
  quantity: number;
}

export type CartStatus = number;

export interface CGM {
  cgmId?: string;
  cgmName?: string;
}

export interface Change {
  changeType: string;
  /** @format date-time */
  timeStamp: string;
  /** @format int64 */
  version: number;
  data: any;
}

export interface ChangeItemQuantity {
  productId: string;
  /** @format int32 */
  quantity: number;
}

export type CheapestBItemDto = {
  bArticleNumber?: string;
  /** @format double */
  bPrice?: number;
  /** @format double */
  bPriceExVat?: number;
  bTitle?: string;
} | null;

export type CheckoutError = {
  message?: string;
  type?: CheckoutErrorType;
} | null;

export type CheckoutErrorType = number;

export interface CreateCartRequest {
  operatingChain: string;
}

export interface CreatePurchaseOrderRequest {
  /** @format uuid */
  cartId: string;
}

export interface DepartmentStockDto {
  departmentID?: string;
  displayStockGEO?: string;
  departmentStock?: boolean;
  timestamp?: string;
}

export interface GlobalTradeItemNumber {
  gtin?: string;
  gtinType?: string;
  packagingUnit?: string;
}

export interface GlobalTradeItemNumbersDto {
  gtin?: string;
  type?: string;
  packaging_unit?: string;
}

export interface GrainResultOfCartDataAndCartError {
  data?: CartData;
  error?: CartError;
  isSuccess?: boolean;
  isFailure?: boolean;
}

export interface GrainResultOfPurchaseOrderAndCheckoutError {
  data?: PurchaseOrder2;
  error?: CheckoutError;
  isSuccess?: boolean;
  isFailure?: boolean;
}

export interface GrainResultOfPurchaseOrderDataAndCheckoutError {
  data?: PurchaseOrderData;
  error?: CheckoutError;
  isSuccess?: boolean;
  isFailure?: boolean;
}

export interface KeyWord {
  id?: string;
}

export type OnlineInfo = {
  relevant?: boolean;
  salesStatus?: string;
} | null;

export interface PFT {
  pftId?: string;
}

export type PresaleInfo = {
  /** @format date-time */
  presaleDate?: string;
  presaleQuantity?: string;
} | null;

export interface Product {
  id: string;
  name: string;
  /** @format double */
  price: number;
  /** @format double */
  priceExVat: number;
  /** @format double */
  beforePrice: number | null;
  /** @format double */
  beforePriceExVat: number | null;
  url: string;
  primaryImageUrl: string;
}

export interface Product2 {
  id: string;
  name: string;
  /** @format double */
  price: number;
  /** @format double */
  priceExVat: number;
  url: string;
  primaryImageUrl: string;
}

export interface ProductAttribute {
  dataType?: string;
  description?: string;
  identifier?: string;
  name?: string;
  presetValueIdentifier?: string;
  purpose?: string;
  /** @format int32 */
  sequence?: number;
  structure?: string;
  value?: string;
}

export interface ProductDetail {
  aItem?: ProductShortItem;
  price?: ProductPrice;
  advertisingText?: string;
  alternativeArticleNumber?: string;
  brand?: Brand;
  cgm?: CGM;
  pft?: PFT;
  pt?: PT;
  articleNumber?: string;
  articleRole?: string;
  articleType?: string | null;
  attributes?: ProductAttribute[];
  bGrade?: BGrade;
  badgeUrl?: string;
  beforePrice?: ProductPrice;
  averageRating?: string | null;
  bulletpoints?: string[];
  chainPrice?: ProductPrice2;
  cheapestBItem?: ProductShortItem2;
  collectAtStore?: boolean;
  currency?: string | null;
  customerClubPrice?: ProductPrice;
  stockInfo?: StockInfo[];
  disclaimer?: string | null;
  globalTradeItemNumbers?: GlobalTradeItemNumber[];
  homeDelivery?: string;
  imageUrl?: string | null;
  sellabillityInfo?: SellabillityInfo;
  mainLogisticalFlow?: string | null;
  manufacturerArticleNumber?: string | null;
  /** @format double */
  marginPercentage?: number | null;
  name?: string | null;
  onlineInfo?: OnlineInfo;
  operatingChain?: string;
  operatingChainSpecificStatus?: string;
  productTaxonomies?: ProductTaxonomy[];
  presaleInfo?: PresaleInfo;
  productType?: string | null;
  productUrl?: string | null;
  shortDescription?: string;
  status?: string | null;
  title?: string;
  vendorArticleNumber?: string;
  vendorGroup?: string;
  id?: string;
}

export interface ProductDto {
  operatingChain?: string;
  bulletpoint1?: string;
  bulletpoint2?: string;
  bulletpoint3?: string;
  title?: string;
  operatingChainSpecificStatus?: string;
  shortDescription?: string;
  isBuyableOnline?: boolean;
  isBuyableInternet?: boolean;
  isBuyableCollectAtStore?: boolean;
  isBuyableInStore?: boolean;
  chainPriceAmount?: string | null;
  chainPriceAmountExVat?: string | null;
  activePriceAmount?: string | null;
  activePriceAmountExVat?: string | null;
  marginPercentage?: string | null;
  currency?: string | null;
  manufacturerArticleNumber?: string | null;
  name?: string | null;
  articleType?: string | null;
  status?: string | null;
  mainLogisticalFlow?: string | null;
  contentStatus?: string | null;
  onlineArticle?: string | null;
  planner?: string | null;
  stockControlCode?: string | null;
  oem?: boolean;
  warranty?: boolean;
  vendorGroup?: string;
  globalTradeItemNumbers?: GlobalTradeItemNumbersDto[];
  articleAssignmentsBrandId?: string;
  articleAssignmentsBrandName?: string;
  articleAssignmentsPTId?: string;
  articleAssignmentsPTName?: string;
  articleAssignmentsCGMId?: string;
  articleAssignmentsCGMName?: string;
  articleAssignmentsPFTId?: string;
  articleAssignmentsTemplateId?: string;
  badgeUrl?: string;
  badgeUrlB2B?: string;
  savePrice?: string;
  advertisingText?: string;
  advertisingTextB2B?: string;
  beforePrice?: string | null;
  beforePriceExVat?: string | null;
  disclaimer?: string | null;
  salesPoint?: string | null;
  salesPointB2B?: string | null;
  bGradeTitle?: string | null;
  bGrade?: string | null;
  presaleDate?: string | null;
  releaseDate?: string | null;
  alwaysOnline?: string;
  articleOperatingChainAttributes?: string;
  collectAtStore?: boolean | null;
  competitionCharacter?: string;
  customerType?: string;
  discountable?: boolean | null;
  displayDate?: string | null;
  displayGrade?: string;
  homeDelivery?: string;
  presaleQuantity?: string;
  providerCode?: string;
  readyToGo?: string;
  retailFirstSalesDate?: string | null;
  returnPolicy?: string;
  serviceType?: string;
  shipToCustomer?: string;
  shipToStore?: string | null;
  stockGrade?: string;
  supplyOverride?: string;
  vendorArticleNumber?: string;
  whlSaleFirstSalesDate?: string | null;
  whlSaleSalesStatus?: string;
  averageRating?: string | null;
  /** @format int32 */
  totalReviewCount?: number | null;
  articleNumber?: string;
  availableFastDelivery?: boolean;
  wholesaleStockFilled?: boolean | null;
  wholeSaleIncomingDate?: string | null;
  wholeSaleDisplayStock?: string | null;
  productURL?: string | null;
  parentArticleNumber?: string | null;
  onlineSalesStatus?: string | null;
  attributes?: AttributeDto[];
  altArticleNumber?: string;
  keyWords?: KeyWord[];
  articleCategory?: string;
  boxSize?: string;
  ptLevel0?: string;
  ptLevel1?: string;
  ptLowestLevelNodeValue?: string;
  departmentStock?: DepartmentStockDto[];
  articleRole?: string;
  retailItemCategoryGroup?: string;
  retailSalesStatus?: string;
  whlSaleItemCategoryGroup?: string;
  productTaxonomy?: string[];
  productTaxonomyId?: string[];
  onlineRelevant?: boolean | null;
  onlineRelevantB2B?: boolean | null;
  stockLastUpdated?: string | null;
  sellerName?: string;
  cheapestBItem?: CheapestBItemDto;
  aItem?: AItemDto;
  image?: string | null;
  productType?: string | null;
}

export type ProductPrice = {
  /** @format double */
  price?: number;
  /** @format double */
  priceExclVat?: number;
} | null;

export interface ProductPrice2 {
  /** @format double */
  price?: number;
  /** @format double */
  priceExclVat?: number;
}

export type ProductShortItem = {
  articleNumber?: string;
  title?: string;
  price?: ProductPrice;
} | null;

export type ProductShortItem2 = {
  articleNumber?: string;
  title?: string;
  price?: any;
} | null;

export interface ProductTaxonomy {
  taxonomyId?: string;
  taxonomyName?: string;
}

export interface PT {
  ptId?: string;
  ptName?: string;
}

export interface PurchaseOrder {
  /** @format uuid */
  id: string;
  items: PurchaseOrderItem[];
  /** @format double */
  total: number;
  /** @format double */
  totalExVat: number;
  currency: string;
  operatingChain: string;
  /** @format uuid */
  cartId: string;
  sessionId: string;
  userId: string | null;
  /**
   * @format int32
   * @default 1
   */
  version?: number;
}

export type PurchaseOrder2 = {
  /** @format uuid */
  id: string;
  items: PurchaseOrderItem[];
  /** @format double */
  total: number;
  /** @format double */
  totalExVat: number;
  currency: string;
  operatingChain: string;
  /** @format uuid */
  cartId: string;
  sessionId: string;
  userId: string | null;
  /**
   * @format int32
   * @default 1
   */
  version?: number;
};

export type PurchaseOrderData = {
  /** @format uuid */
  id: string;
  /** @format int32 */
  version: number;
  items: PurchaseOrderItem[];
  /** @format double */
  total: number;
  /** @format double */
  totalExVat: number;
  currency: string;
  operatingChain: string;
  sessionId: string;
  userId: string | null;
  /** @format uuid */
  cartId: string;
};

export interface PurchaseOrderItem {
  product: Product2;
  /** @format int32 */
  quantity: number;
}

export interface SellabillityInfo {
  collectAtStore?: boolean;
  inStore?: boolean;
  online?: boolean;
}

export interface StockInfo {
  departmentId?: string;
  departmentStock?: boolean;
  displayStockGeo?: string;
  /** @format date-time */
  timeStamp?: string;
}
