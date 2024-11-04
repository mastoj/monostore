namespace MonoStore.Product.Contracts;

[GenerateSerializer, Alias(nameof(GlobalTradeItemNumbersDto))]
public record class GlobalTradeItemNumbersDto
{
  [Id(0)]
  public string gtin { get; set; } = "";
  [Id(1)]
  public string type { get; set; } = "";
  [Id(2)]
  public string packaging_unit { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(AttributeDto))]
public record class AttributeDto
{
  [Id(0)]
  public string Name { get; set; } = "";
  [Id(1)]
  public string Purpose { get; set; } = "";
  [Id(2)]
  public int Sequence { get; set; } = 0;
  [Id(3)]
  public string Value { get; set; } = "";
  [Id(4)]
  public string Structure { get; set; } = "";
  [Id(5)]
  public string Identifier { get; set; } = "";
  [Id(6)]
  public string Description { get; set; } = "";
  [Id(7)]
  public string PresetValueIdentifier { get; set; } = "";
  [Id(8)]
  public string DataType { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(KeyWord))]
public record class KeyWord
{
  [Id(0)]
  public string ID { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(DepartmentStockDto))]
public record class DepartmentStockDto
{
  [Id(0)]
  public string DepartmentID { get; set; } = "";
  [Id(1)]
  public string DisplayStockGEO { get; set; } = "";
  [Id(2)]
  public bool DepartmentStock { get; set; }
  [Id(3)]
  public string Timestamp { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(CheapestBItemDto))]
public record class CheapestBItemDto
{
  [Id(0)]
  public string BArticleNumber { get; set; } = "";
  [Id(1)]
  public decimal BPrice { get; set; } = 0;
  [Id(2)]
  public decimal BPriceExVat { get; set; } = 0;
  [Id(3)]
  public string BTitle { get; set; } = "";

}

[GenerateSerializer, Alias(nameof(AItemDto))]
public record class AItemDto
{
  [Id(0)]
  public string AArticleNumber { get; set; } = "";
  [Id(1)]
  public decimal? APrice { get; set; }
  [Id(2)]
  public decimal? APriceExVat { get; set; }
  [Id(3)]
  public string ATitle { get; set; } = "";

}

[GenerateSerializer, Alias(nameof(ProductDto))]
public record class ProductDto
{
  [Id(0)]
  public string OperatingChain { get; set; } = "";
  [Id(1)]
  public string Bulletpoint1 { get; set; } = "";
  [Id(2)]
  public string Bulletpoint2 { get; set; } = "";
  [Id(3)]
  public string Bulletpoint3 { get; set; } = "";
  [Id(4)]
  public string Title { get; set; } = "";
  [Id(5)]
  public string OperatingChainSpecificStatus { get; set; } = "";
  [Id(6)]
  public string ShortDescription { get; set; } = "";
  [Id(7)]
  public bool IsBuyableOnline { get; set; }
  [Id(8)]
  public bool IsBuyableInternet { get; set; }
  [Id(9)]
  public bool IsBuyableCollectAtStore { get; set; }
  [Id(10)]
  public bool IsBuyableInStore { get; set; }
  [Id(11)]
  public string? ChainPriceAmount { get; set; }
  [Id(12)]
  public string? ChainPriceAmountExVat { get; set; }
  [Id(13)]
  public string? ActivePriceAmount { get; set; }
  [Id(14)]
  public string? ActivePriceAmountExVat { get; set; }
  [Id(15)]
  public string? MarginPercentage { get; set; }
  [Id(16)]
  public string? Currency { get; set; }
  [Id(17)]
  public string? ManufacturerArticleNumber { get; set; }
  [Id(18)]
  public string? Name { get; set; }
  [Id(19)]
  public string? ArticleType { get; set; }
  [Id(20)]
  public string? Status { get; set; }
  [Id(21)]
  public string? MainLogisticalFlow { get; set; }
  [Id(22)]
  public string? ContentStatus { get; set; }
  [Id(23)]
  public string? OnlineArticle { get; set; }
  [Id(24)]
  public string? Planner { get; set; }
  [Id(25)]
  public string? StockControlCode { get; set; }
  [Id(26)]
  public bool OEM { get; set; }
  [Id(27)]
  public bool Warranty { get; set; }
  [Id(28)]
  public string VendorGroup { get; set; } = "";
  [Id(29)]
  public GlobalTradeItemNumbersDto[] GlobalTradeItemNumbers { get; set; } = Array.Empty<GlobalTradeItemNumbersDto>();
  [Id(30)]
  public string ArticleAssignmentsBrandId { get; set; } = "";
  [Id(31)]
  public string ArticleAssignmentsBrandName { get; set; } = "";
  [Id(32)]
  public string ArticleAssignmentsPTId { get; set; } = "";
  [Id(33)]
  public string ArticleAssignmentsPTName { get; set; } = "";
  [Id(34)]
  public string ArticleAssignmentsCGMId { get; set; } = "";
  [Id(35)]
  public string ArticleAssignmentsCGMName { get; set; } = "";
  [Id(36)]
  public string ArticleAssignmentsPFTId { get; set; } = "";
  [Id(37)]
  public string ArticleAssignmentsTemplateId { get; set; } = "";
  [Id(38)]
  public string BadgeUrl { get; set; } = "";
  [Id(39)]
  public string BadgeUrlB2B { get; set; } = "";
  [Id(40)]
  public string SavePrice { get; set; } = "";
  [Id(41)]
  public string AdvertisingText { get; set; } = "";
  [Id(42)]
  public string AdvertisingTextB2B { get; set; } = "";
  [Id(43)]
  public string? BeforePrice { get; set; }
  [Id(44)]
  public string? BeforePriceExVat { get; set; }
  [Id(45)]
  public string? Disclaimer { get; set; }
  [Id(46)]
  public string? SalesPoint { get; set; }
  [Id(47)]
  public string? SalesPointB2B { get; set; }
  [Id(48)]
  public string? BGradeTitle { get; set; }
  [Id(49)]
  public string? BGrade { get; set; }
  [Id(50)]
  public string? PresaleDate { get; set; }
  [Id(51)]
  public string? ReleaseDate { get; set; }
  [Id(52)]
  public string AlwaysOnline { get; set; } = "";
  [Id(53)]
  public string ArticleOperatingChainAttributes { get; set; } = "";
  [Id(54)]
  public bool? CollectAtStore { get; set; }
  [Id(55)]
  public string CompetitionCharacter { get; set; } = "";
  [Id(56)]
  public string CustomerType { get; set; } = "";
  [Id(57)]
  public bool? Discountable { get; set; }
  [Id(58)]
  public string? DisplayDate { get; set; }
  [Id(59)]
  public string DisplayGrade { get; set; } = "";
  [Id(60)]
  public string HomeDelivery { get; set; } = "";
  [Id(61)]
  public string PresaleQuantity { get; set; } = "";
  [Id(62)]
  public string ProviderCode { get; set; } = "";
  [Id(63)]
  public string ReadyToGo { get; set; } = "";
  [Id(64)]
  public string? RetailFirstSalesDate { get; set; }
  [Id(65)]
  public string ReturnPolicy { get; set; } = "";
  [Id(66)]
  public string ServiceType { get; set; } = "";
  [Id(67)]
  public string ShipToCustomer { get; set; } = "";
  [Id(68)]
  public string? ShipToStore { get; set; }
  [Id(69)]
  public string StockGrade { get; set; } = "";
  [Id(70)]
  public string SupplyOverride { get; set; } = "";
  [Id(71)]
  public string VendorArticleNumber { get; set; } = "";
  [Id(72)]
  public string? WhlSaleFirstSalesDate { get; set; }
  [Id(73)]
  public string WhlSaleSalesStatus { get; set; } = "";
  [Id(74)]
  public string? AverageRating { get; set; }
  [Id(75)]
  public int? TotalReviewCount { get; set; }
  [Id(76)]
  public string ArticleNumber { get; set; } = "";
  [Id(77)]
  public bool AvailableFastDelivery { get; set; }
  [Id(78)]
  public bool? WholesaleStockFilled { get; set; }
  [Id(79)]
  public string? WholeSaleIncomingDate { get; set; }
  [Id(80)]
  public string? WholeSaleDisplayStock { get; set; }
  [Id(81)]
  public string? ProductURL { get; set; }
  [Id(82)]
  public string? ParentArticleNumber { get; set; }
  [Id(83)]
  public string? OnlineSalesStatus { get; set; }
  [Id(84)]
  public AttributeDto[] Attributes { get; set; } = Array.Empty<AttributeDto>();
  [Id(85)]
  public string AltArticleNumber { get; set; } = "";
  [Id(86)]
  public KeyWord[] KeyWords { get; set; } = Array.Empty<KeyWord>();
  [Id(87)]
  public string ArticleCategory { get; set; } = "";
  [Id(88)]
  public string BoxSize { get; set; } = "";
  [Id(89)]
  public string PTLevel0 { get; set; } = "";
  [Id(90)]
  public string PTLevel1 { get; set; } = "";
  [Id(91)]
  public string PTLowestLevelNodeValue { get; set; } = "";
  [Id(92)]
  public DepartmentStockDto[] DepartmentStock { get; set; } = Array.Empty<DepartmentStockDto>();
  [Id(93)]
  public string ArticleRole { get; set; } = "";
  [Id(94)]
  public string RetailItemCategoryGroup { get; set; } = "";
  [Id(95)]
  public string RetailSalesStatus { get; set; } = "";
  [Id(96)]
  public string WhlSaleItemCategoryGroup { get; set; } = "";
  [Id(97)]
  public string[] ProductTaxonomy { get; set; } = Array.Empty<string>();
  [Id(98)]
  public string[] ProductTaxonomyId { get; set; } = Array.Empty<string>();
  [Id(99)]
  public bool? OnlineRelevant { get; set; }
  [Id(100)]
  public bool? OnlineRelevantB2B { get; set; }
  [Id(101)]
  public string? StockLastUpdated { get; set; }
  [Id(102)]
  public string SellerName { get; set; } = "";
  [Id(103)]
  public CheapestBItemDto? CheapestBItem { get; set; }
  [Id(104)]
  public AItemDto? AItem { get; set; }
  [Id(105)]
  public string? Image { get; set; }
  [Id(106)]
  public string? ProductType { get; set; }
}