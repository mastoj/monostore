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

public static class CartEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Cart.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");


  public static string CartGrainId(string cartId) => $"cart/{cartId.ToLower()}";
  public static void MapCartEndpoints(this IEndpointRouteBuilder routes)
  {
    routes.MapPost("/", async (IGrainFactory grains, CreateCart createCart) =>
    {
      // DiagnosticConfig.CreateCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
      try
      {
        Console.WriteLine("CreateCart");
        var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(createCart.CartId.ToString()));
        var result = await cartGrain.CreateCart(Guid.Parse(createCart.CartId));
        OperationsCounter.Add(1, new TagList() {
          { "operation", "create" },
          { "status", "success" },
        });
        Console.WriteLine($"Cart created: {result.Id}");
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

    routes.MapPost("/{id}/items", async (IGrainFactory grains, Contracts.AddItem addItem) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(addItem.CartId));
      return await cartGrain.AddItem(addItem);
    });

    routes.MapDelete("/{id}/items/{productId}", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id.ToString()));
      return await cartGrain.RemoveItem(new Contracts.RemoveItem
      {
        CartId = id.ToString(),
        ProductId = productId
      });
    });

    routes.MapPost("/{id}/items/{productId}/increase", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id.ToString()));
      return await cartGrain.IncreaseItemQuantity(new Contracts.IncreaseItemQuantity
      {
        CartId = id.ToString(),
        ProductId = productId
      });
    });

    routes.MapPost("/{id}/items/{productId}/decrease", async (IGrainFactory grains, Guid id, string productId) =>
    {
      var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id.ToString()));
      return await cartGrain.DecreaseItemQuantity(new Contracts.DecreaseItemQuantity
      {
        CartId = id.ToString(),
        ProductId = productId
      });
    });

    routes.MapGet("/{id}", async (IGrainFactory grains, string id) =>
    {
      try
      {
        OperationsCounter.Add(1, new TagList() {
          { "operation", "get" },
          { "status", "success" },
        });

        var cartGrain = grains.GetGrain<ICartGrain>(CartGrainId(id));
        return await cartGrain.GetCart(Guid.Parse(id));
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

    // routes.MapGet("/", async (IGrainFactory grains) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart");
    //   return await cartGrain.GetCart("1");
    // });

    // routes.MapGet("/{id}", async (IGrainFactory grains, int id) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart" + id);
    //   return await cartGrain.GetCart(id.ToString());
    // });

    // routes.MapPost("/", async (IGrainFactory grains, AddItem addItem) =>
    // {
    //   var cartGrain = grains.GetGrain<ICartGrain>("cart");
    //   var cart = await cartGrain.GetCart(addItem.CartId);
    //   return cart;
    // });
    // dotnet add package Microsoft.AspNetCore.App
  }

  public static WebApplication UseCart(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCartEndpoints();
    return app;
  }
}