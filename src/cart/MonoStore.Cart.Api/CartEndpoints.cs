namespace MonoStore.Cart.Api;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using MonoStore.Cart.Contracts.Grains;
using Orleans;
using MonoStore.Product.Contracts.Grains;
using MonoStore.Cart.Contracts.Requests;
using Microsoft.Extensions.Logging;

public static class CartEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Cart.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");

  public static RouteGroupBuilder MapCartEndpoints(this RouteGroupBuilder routes)
  {
    routes.MapPost("/", async (HttpRequest request, IGrainFactory grains, CreateCartRequest createCart, ILoggerFactory loggerFactory) =>
    {
      var logger = loggerFactory.CreateLogger("CartEndpoints");
      if (!request.Cookies.TryGetValue("session-id", out var sessionId))
      {
        return Results.BadRequest("Missing session-id cookie");
      }

      request.Cookies.TryGetValue("user-id", out var userId);
      var cartId = Guid.NewGuid();
      // DiagnosticConfig.CreateCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
      try
      {
        Console.WriteLine("CreateCart");
        var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(cartId));
        var result = await cartGrain.CreateCart(new CreateCartMessage(cartId, createCart.OperatingChain, sessionId, userId));
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "success" },
        });
        return Results.Ok(result);
      }
      catch (Exception ex)
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "failure" },
        });
        logger.LogError(ex, "Error creating cart");
        return Results.Problem(ex.Message);
      }
    });

    routes.MapPost("/{id}/items", async (IGrainFactory grains, Guid id, AddItemRequest addItemRequest) =>
    {
      var productGrainId = IProductGrain.ProductGrainId(addItemRequest.OperatingChain, addItemRequest.ProductId);
      Console.WriteLine($"==> ProductGrainId: {productGrainId}");
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.AddItem(addItemRequest);
    });

    routes.MapDelete("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.RemoveItem(new RemoveItem(productId));
    });

    routes.MapPut("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId, ChangeItemQuantity changeItemQuantity) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.ChangeItemQuantity(new ChangeItemQuantity(productId, changeItemQuantity.Quantity));
    });

    routes.MapGet("/{id}", async (IGrainFactory grains, Guid id) =>
    {
      try
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "get" },
          { "status", "success" },
        });

        var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
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
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.AbandonCart(new AbandonCart());
    });

    routes.MapPost("/{id}/clear", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.ClearCart(new ClearCart());
    });

    routes.MapPost("/{id}/recover", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
      return await cartGrain.RecoverCart(new RecoverCart());
    });

    routes.MapPost("/{id}/archive", async (IGrainFactory grains, Guid id) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(id));
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