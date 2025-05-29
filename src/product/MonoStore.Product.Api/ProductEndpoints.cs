using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MonoStore.Contracts.Product;
using MonoStore.Contracts.Product.Grains;

namespace MonoStore.Product.Api;

public static class ProductEndpoints
{
  public static RouteGroupBuilder MapProductEndpoints(this RouteGroupBuilder routes)
  {
    routes.MapGet("/{operatingChain}/{id}", async (IGrainFactory grains, string operatingChain, string id) =>
    {
      var productGrainId = IProductGrain.ProductGrainId(operatingChain, id);
      Console.WriteLine($"==> ProductGrainId: {productGrainId}");
      var productGrain = grains.GetGrain<IProductGrain>(productGrainId);
      return await productGrain.GetProductAsync();
    });
    routes.MapPost("/", async (IGrainFactory grains, ProductDetail product) =>
    {
      throw new NotImplementedException();
      // var productGrain = grains.GetGrain<IProductGrain>(product.Sku);
      // return await productGrain.UpdateProductAsync(product);
    });
    // routes.MapPost("/sync", async (IGrainFactory grains, ProductDto[] products) =>
    routes.MapPost("/sync", async (IGrainFactory grains, [FromBody] ProductDto[] productDtos) =>
    {
      // var productDtos = await request.ReadFromJsonAsync<ProductDto[]>();
      Console.WriteLine("==> ProductDtos: " + productDtos.Length);
      var productDetails = productDtos.Select(Mappers.MapProductDto).ToList();
      Console.WriteLine("==> Syncing products: " + productDetails.Count);
      var productSyncGrain = grains.GetGrain<IProductSyncGrain>(IProductSyncGrain.ProductSyncGrainId());
      await productSyncGrain.SyncProductAsync(productDetails);
      return Results.Ok();
    });
    return routes;
  }

  public static WebApplication UseProduct(this WebApplication app, string groupPath)
  {
    Console.WriteLine("Adding UseProduct");
    app.MapGroup(groupPath).MapProductEndpoints().WithTags("product");
    return app;
  }
}
