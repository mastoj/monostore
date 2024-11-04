namespace MonoStore.Product.Contracts;

[GenerateSerializer, Alias(nameof(ProductPrice))]
public record class ProductPrice
{
  [Id(0)]
  public decimal Price { get; set; } = 0;
  [Id(1)]
  public decimal PriceExclVat { get; set; } = 0;
}

[GenerateSerializer, Alias(nameof(ProductShortItem))]
public record class ProductShortItem
{
  [Id(0)]
  public string ArticleNumber { get; set; } = "";
  [Id(1)]
  public string Title { get; set; } = "";
  [Id(2)]
  public ProductPrice? Price { get; set; } = new ProductPrice();
}

[GenerateSerializer, Alias(nameof(Brand))]
public record class Brand
{
  [Id(0)]
  public string BrandId { get; set; } = "";
  [Id(1)]
  public string BrandName { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(CGM))]
public record class CGM
{
  [Id(0)]
  public string CGMId { get; set; } = "";
  [Id(1)]
  public string CGMName { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(PT))]
public record class PT
{
  [Id(0)]
  public string PTId { get; set; } = "";
  [Id(1)]
  public string PTName { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(PFT))]
public record class PFT
{
  [Id(0)]
  public string PFTId { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(ProductAttribute))]
public record class ProductAttribute
{
  [Id(0)]
  public string DataType { get; set; } = "";
  [Id(1)]
  public string Description { get; set; } = "";
  [Id(2)]
  public string Identifier { get; set; } = "";
  [Id(3)]
  public string Name { get; set; } = "";
  [Id(4)]
  public string PresetValueIdentifier { get; set; } = "";
  [Id(5)]
  public string Purpose { get; set; } = "";
  [Id(6)]
  public int Sequence { get; set; } = 0;
  [Id(7)]
  public string Structure { get; set; } = "";
  [Id(8)]
  public string Value { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(BGrade))]
public record class BGrade
{
  [Id(0)]
  public string BGradeId { get; set; } = "";
  [Id(1)]
  public string BGradeTitle { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(StockInfo))]
public record class StockInfo
{
  [Id(0)]
  public string DepartmentId { get; set; } = "";
  [Id(1)]
  public bool DepartmentStock { get; set; }
  [Id(2)]
  public string DisplayStockGeo { get; set; } = "";
  [Id(3)]
  public DateTimeOffset TimeStamp { get; set; }
}

[GenerateSerializer, Alias(nameof(GlobalTradeItemNumber))]
public record class GlobalTradeItemNumber
{
  [Id(0)]
  public string GTIN { get; set; } = "";
  [Id(1)]
  public string GTINType { get; set; } = "";
  [Id(2)]
  public string PackagingUnit { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(SellabillityInfo))]
public record class SellabillityInfo
{
  [Id(0)]
  public bool CollectAtStore { get; set; }
  [Id(1)]
  public bool InStore { get; set; }
  [Id(2)]
  public bool Online { get; set; }
}

[GenerateSerializer, Alias(nameof(OnlineInfo))]
public record class OnlineInfo
{
  [Id(0)]
  public bool Relevant { get; set; }
  [Id(1)]
  public string SalesStatus { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(ProductTaxonomy))]
public record class ProductTaxonomy
{
  [Id(0)]
  public string TaxonomyId { get; set; } = "";
  [Id(1)]
  public string TaxonomyName { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(PresaleInfo))]
public record class PresaleInfo
{
  [Id(0)]
  public DateTimeOffset PresaleDate { get; set; }
  [Id(1)]
  public string PresaleQuantity { get; set; } = "";
}

[GenerateSerializer, Alias(nameof(ProductDetail))]
public record class ProductDetail
{
  [Id(0)]
  public ProductShortItem? AItem { get; set; }
  [Id(1)]
  public ProductPrice? Price { get; set; } = new ProductPrice();
  [Id(2)]
  public string AdvertisingText { get; set; } = "";
  [Id(3)]
  [Obsolete]
  public string AlternativeArticleNumber { get; set; } = "";
  [Id(4)]
  public Brand Brand { get; set; } = new Brand();
  [Id(5)]
  public CGM CGM { get; set; } = new CGM();
  [Id(6)]
  public PFT PFT { get; set; } = new PFT();
  [Id(7)]
  public PT PT { get; set; } = new PT();
  [Id(8)]
  public string ArticleNumber { get; set; } = "";
  [Id(9)]
  public string ArticleRole { get; set; } = "";
  [Id(10)]
  public string? ArticleType { get; set; }
  [Id(11)]
  public ProductAttribute[] Attributes { get; set; } = Array.Empty<ProductAttribute>();
  [Id(12)]
  public BGrade? BGrade { get; set; }
  [Id(13)]
  public string BadgeUrl { get; set; } = "";
  [Id(14)]
  public ProductPrice? BeforePrice { get; set; }
  [Id(15)]
  public string? AverageRating { get; set; }
  [Id(16)]
  public string[] Bulletpoints { get; set; } = Array.Empty<string>();
  [Id(17)]
  public ProductPrice ChainPrice { get; set; } = new ProductPrice();
  [Id(18)]
  public ProductShortItem? CheapestBItem { get; set; }
  [Id(19)]
  public bool CollectAtStore { get; set; }
  [Id(20)]
  public string? Currency { get; set; } = "";
  [Id(21)]
  public ProductPrice? CustomerClubPrice { get; set; }
  [Id(22)]
  public StockInfo[] StockInfo { get; set; } = Array.Empty<StockInfo>();
  [Id(23)]
  public string? Disclaimer { get; set; }
  // Energyrating
  [Id(24)]
  public GlobalTradeItemNumber[] GlobalTradeItemNumbers { get; set; } = Array.Empty<GlobalTradeItemNumber>();
  [Id(25)]
  public string HomeDelivery { get; set; } = "";
  [Id(26)]
  public string? ImageUrl { get; set; }
  [Id(27)]
  public SellabillityInfo SellabillityInfo { get; set; } = new SellabillityInfo();
  [Id(28)]
  public string? MainLogisticalFlow { get; set; }
  [Id(29)]
  public string? ManufacturerArticleNumber { get; set; }
  [Id(30)]
  public decimal? MarginPercentage { get; set; }
  [Id(31)]
  public string? Name { get; set; }
  [Id(32)]
  public OnlineInfo? OnlineInfo { get; set; }
  [Id(33)]
  public string OperatingChain { get; set; } = "";
  [Id(34)]
  public string OperatingChainSpecificStatus { get; set; } = "";
  [Id(35)]
  public ProductTaxonomy[] ProductTaxonomies { get; set; } = Array.Empty<ProductTaxonomy>();
  [Id(36)]
  public PresaleInfo? PresaleInfo { get; set; }
  [Id(37)]
  public string? ProductType { get; set; } = "";
  [Id(38)]
  public string? ProductUrl { get; set; } = "";
  [Id(39)]
  public string ShortDescription { get; set; } = "";
  [Id(40)]
  public string? Status { get; set; } = "";
  [Id(41)]
  public string Title { get; set; } = "";
  [Id(42)]
  public string VendorArticleNumber { get; set; } = "";
  [Id(43)]
  public string VendorGroup { get; set; } = "";
}


public interface IProductService
{
  Task<ProductDetail> GetProductAsync(string sku, string operatingChain);
}