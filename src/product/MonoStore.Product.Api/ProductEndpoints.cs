using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MonoStore.Product.Contracts;
using MonoStore.Product.Contracts.Grains;

namespace MonoStore.Product.Api;

public static class ProductEndpoints
{
  private static string ProductGrainId(string operatingChain, string id) => $"product/{operatingChain.ToLower()}_{id.ToLower()}";

  public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapGet("/{operatingChain}/{id}", async (IGrainFactory grains, string operatingChain, string id) =>
    {
      var productGrainId = ProductGrainId(operatingChain, id);
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
    routes.MapPost("/sync", async (IGrainFactory grains, HttpRequest request) =>
    {
      string body = "";
      using (StreamReader stream = new StreamReader(request.Body))
      {
        body = await stream.ReadToEndAsync();
      }
      Console.WriteLine("==> Syncing products", body);
      var productDtos = JsonSerializer.Deserialize<ProductDto[]>(body);
      Console.WriteLine("==> Syncing products", productDtos);
      return Results.Json(body); ;
      // foreach (var product in products)
      // {
      //   var productGrain = grains.GetGrain<IProductGrain>(product.Sku);
      //   await productGrain.UpdateProductAsync(product);
      // }
      // return products;
    });
  }

  public static WebApplication UseProduct(this WebApplication app, string groupPath)
  {
    Console.WriteLine("Adding UseProduct");
    app.MapGroup(groupPath).MapProductEndpoints();
    return app;
  }
}
