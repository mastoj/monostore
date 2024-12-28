namespace MonoStore.Cart.Module;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Monostore.ServiceDefaults;
using MonoStore.Cart.Contracts;
using MonoStore.Cart.Contracts.Grains;
using Orleans;
using MonoStore.Product.Contracts.Grains;
using Microsoft.AspNetCore.Http;
using MonoStore.Cart.Contracts.Requests;

public static class CartEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Cart.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");


  public static string CartGrainId(Guid cartId) => $"cart/{cartId.ToString().ToLower()}";
  public static RouteGroupBuilder MapCartEndpoints(this RouteGroupBuilder routes)
  {
    routes.MapPost("/", async (IGrainFactory grains, CreateCart createCart) =>
    {
      // DiagnosticConfig.CreateCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
      try
      {
        Console.WriteLine("CreateCart");
        var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(createCart.CartId));
        var result = await cartGrain.CreateCart(createCart);
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "success" },
        });
        Console.WriteLine($"Cart created: {result?.Data?.Id}");
        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error creating cart");
        Console.WriteLine(ex.Message);
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "error" },
        });
        throw;
      }
    });

    routes.MapPost("/{id}/items", async (IGrainFactory grains, Guid id, AddItemRequest addItemRequest) =>
    {
      var productGrainId = IProductGrain.ProductGrainId(addItemRequest.OperatingChain, addItemRequest.ProductId);
      Console.WriteLine($"==> ProductGrainId: {productGrainId}");
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.AddItem(addItemRequest);
    });

    routes.MapDelete("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.RemoveItem(new RemoveItem(productId));
    });

    routes.MapPost("/{id}/items/{productId}/increase", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.IncreaseItemQuantity(new IncreaseItemQuantity(productId));
    });

    routes.MapPost("/{id}/items/{productId}/decrease", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.DecreaseItemQuantity(new DecreaseItemQuantity(productId));
    });

    routes.MapGet("/{id}", async (IGrainFactory grains, Guid id) =>
    {
      try
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "get" },
          { "status", "success" },
        });

        var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
        return await cartGrain.GetCart(new GetCart());
      }
      catch (Exception ex)
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "get" },
          { "status", "success" },
        });

        Console.WriteLine(ex.Message);
        throw;
      }
    });

    routes.MapPost("/{id}/abandon", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.AbandonCart(new AbandonCart());
    });

    routes.MapPost("/{id}/clear", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.ClearCart(new ClearCart());
    });

    routes.MapPost("/{id}/recover", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.RecoverCart(new RecoverCart());
    });

    routes.MapPost("/{id}/archive", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
      return await cartGrain.ArchiveCart(new ArchiveCart());
    });

    return routes;
  }

  public static WebApplication UseCart(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCartEndpoints().WithTags("cart");
    return app;
  }
}