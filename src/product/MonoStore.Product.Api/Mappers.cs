using System.Globalization;
using MonoStore.Contracts.Product;

namespace MonoStore.Product.Api;

public static class Mappers
{

  static ProductAttribute MapAssignmentDto(AttributeDto attributeDto)
  {
    return new ProductAttribute
    {
      Name = attributeDto.Name,
      Purpose = attributeDto.Purpose,
      Sequence = attributeDto.Sequence,
      Value = attributeDto.Value,
      Structure = attributeDto.Structure,
      Identifier = attributeDto.Identifier,
      Description = attributeDto.Description,
      PresetValueIdentifier = attributeDto.PresetValueIdentifier,
      DataType = attributeDto.DataType
    };
  }

  static ProductPrice? MapPriceStrings(string? price, string? priceExclVat)
  {

    if (price != null && priceExclVat != null)
    {
      var invariantPrice = price.Replace(".", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
      var invariantPriceExVat = priceExclVat.Replace(".", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
      return new ProductPrice
      {
        Price = decimal.Parse(invariantPrice, NumberStyles.Any, CultureInfo.InvariantCulture),
        PriceExclVat = decimal.Parse(priceExclVat, NumberStyles.Any, CultureInfo.InvariantCulture)
      };

    }
    return null;
  }

  static ProductPrice? MapPrice(decimal? price, decimal? priceExclVat)
  {
    return price != null && priceExclVat != null ? new ProductPrice
    {
      Price = price ?? 0,
      PriceExclVat = priceExclVat ?? 0,
    } : null; ;
  }

  static StockInfo MapStockInfo(DepartmentStockDto departmentStockDto)
  {
    return new StockInfo
    {
      DepartmentId = departmentStockDto.DepartmentID,
      DepartmentStock = departmentStockDto.DepartmentStock,
      DisplayStockGeo = departmentStockDto.DisplayStockGEO,
      TimeStamp = DateTimeOffset.Parse(departmentStockDto.Timestamp)
    };
  }

  static GlobalTradeItemNumber MapGlobalTradeItemNumber(GlobalTradeItemNumbersDto globalTradeItemNumbersDto)
  {
    return new GlobalTradeItemNumber
    {
      GTIN = globalTradeItemNumbersDto.gtin,
      GTINType = globalTradeItemNumbersDto.type,
      PackagingUnit = globalTradeItemNumbersDto.packaging_unit
    };
  }

  static ProductTaxonomy[] MapProductTaxonomies(string[] productTaxonomy, string[] productTaxonomyId)
  {
    if (productTaxonomy == null || productTaxonomyId == null)
    {
      return Array.Empty<ProductTaxonomy>();
    }
    return productTaxonomy.Zip(productTaxonomyId, (taxonomy, id) => new ProductTaxonomy
    {
      TaxonomyName = taxonomy,
      TaxonomyId = id
    }).ToArray();
  }

  public static ProductDetail MapProductDto(ProductDto productDto)
  {
    try
    {
      return new ProductDetail
      {
        id = $"{productDto.OperatingChain}_{productDto.ArticleNumber}",
        AItem = productDto.AItem != null && productDto.AItem.APrice != null ? new ProductShortItem
        {
          ArticleNumber = productDto.AItem.AArticleNumber,
          Title = productDto.AItem.ATitle,
          Price = new ProductPrice
          {
            Price = productDto.AItem.APrice ?? 0,
            PriceExclVat = productDto.AItem.APriceExVat ?? 0
          }
        } : null,
        Price = MapPriceStrings(productDto.ActivePriceAmount, productDto.ActivePriceAmountExVat),
        AdvertisingText = productDto.AdvertisingText,
        //    AlternativeArticleNumber = productDto.AlternativeArticleNumber,
        Brand = new Brand
        {
          BrandId = productDto.ArticleAssignmentsBrandId,
          BrandName = productDto.ArticleAssignmentsBrandName
        },
        CGM = new CGM
        {
          CGMId = productDto.ArticleAssignmentsCGMId,
          CGMName = productDto.ArticleAssignmentsCGMName
        },
        PFT = new PFT
        {
          PFTId = productDto.ArticleAssignmentsPFTId
        },
        PT = new PT
        {
          PTId = productDto.ArticleAssignmentsPTId,
          PTName = productDto.ArticleAssignmentsPTName
        },
        ArticleNumber = productDto.ArticleNumber,
        ArticleRole = productDto.ArticleRole,
        ArticleType = productDto.ArticleType,
        Attributes = productDto.Attributes != null ? productDto.Attributes.Select(MapAssignmentDto).ToArray() : Array.Empty<ProductAttribute>(),
        BGrade = productDto.BGrade != null && productDto.BGradeTitle != null ? new BGrade
        {
          BGradeId = productDto.BGrade,
          BGradeTitle = productDto.BGradeTitle
        } : null,
        BadgeUrl = productDto.BadgeUrl,
        BeforePrice = MapPriceStrings(productDto.BeforePrice, productDto.BeforePriceExVat),
        AverageRating = productDto.AverageRating,
        Bulletpoints = new string?[] { productDto.Bulletpoint1, productDto.Bulletpoint2, productDto.Bulletpoint3 }.Where(x => x != null).Cast<string>().ToArray(),
        ChainPrice = MapPriceStrings(productDto.ChainPriceAmount, productDto.ChainPriceAmountExVat),
        CheapestBItem = productDto.CheapestBItem != null ? new ProductShortItem
        {
          ArticleNumber = productDto.CheapestBItem.BArticleNumber,
          Title = productDto.CheapestBItem.BTitle,
          Price = MapPrice(productDto.CheapestBItem.BPrice, productDto.CheapestBItem.BPriceExVat)
        } : null,
        CollectAtStore = productDto.IsBuyableCollectAtStore,
        Currency = productDto.Currency,
        // CustomerClubPrice = MapPriceStrings(productDto., productDto.CustomerClubPriceAmountExVat)
        StockInfo = productDto.DepartmentStock != null ? productDto.DepartmentStock.Select(MapStockInfo).ToArray() : Array.Empty<StockInfo>(),
        Disclaimer = productDto.Disclaimer,
        // Energyrating
        GlobalTradeItemNumbers = productDto.GlobalTradeItemNumbers != null ? productDto.GlobalTradeItemNumbers.Select(MapGlobalTradeItemNumber).ToArray() : Array.Empty<GlobalTradeItemNumber>(),
        HomeDelivery = productDto.HomeDelivery,
        ImageUrl = productDto.Image,
        SellabillityInfo = new SellabillityInfo
        {
          CollectAtStore = productDto.IsBuyableCollectAtStore,
          InStore = productDto.IsBuyableInStore,
          Online = productDto.IsBuyableOnline
        },
        MainLogisticalFlow = productDto.MainLogisticalFlow,
        ManufacturerArticleNumber = productDto.ManufacturerArticleNumber,
        MarginPercentage = productDto.MarginPercentage != null ? decimal.Parse(productDto.MarginPercentage) : null,
        Name = productDto.Name,
        OnlineInfo = (productDto.OnlineRelevant is not null) && (productDto.OnlineSalesStatus is not null) ? new OnlineInfo
        {
          Relevant = productDto.OnlineRelevant ?? false,
          SalesStatus = productDto.OnlineSalesStatus ?? ""
        } : null,
        OperatingChain = productDto.OperatingChain,
        OperatingChainSpecificStatus = productDto.OperatingChainSpecificStatus,
        ProductTaxonomies = MapProductTaxonomies(productDto.ProductTaxonomy, productDto.ProductTaxonomyId),
        PresaleInfo = productDto.PresaleDate != null ? new PresaleInfo
        {
          PresaleQuantity = productDto.PresaleQuantity,
          PresaleDate = DateTimeOffset.Parse(productDto.PresaleDate)
        } : null,
        ProductType = productDto.ProductType,
        ProductUrl = productDto.ProductURL,
        ShortDescription = productDto.ShortDescription,
        Status = productDto.Status,
        Title = productDto.Title,
        VendorArticleNumber = productDto.VendorArticleNumber,
        VendorGroup = productDto.VendorGroup,
      };
    }
    catch (Exception e)
    {
      Console.WriteLine("==> Failed with sku: ", productDto.ArticleNumber);
      throw;
    }
  }
}
