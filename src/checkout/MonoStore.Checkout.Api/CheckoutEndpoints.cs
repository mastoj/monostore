namespace MonoStore.Checkout.Api;

using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using MonoStore.Cart.Contracts.Grains;
using Orleans;
// using Microsoft.AspNetCore.Http;
using MonoStore.Checkout.Contracts.Requests;
using Monostore.Checkout.Contracts.Grains;
using MonoStore.Checkout.Contracts;
using MonoStore.Cart.Contracts.Requests;

public static class CheckoutEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Checkout.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");


  public static RouteGroupBuilder MapCheckoutEndpoints(this RouteGroupBuilder routes)
  {
    routes.MapPost("/", async (IGrainFactory grains, CreatePurchaseOrderRequest createPurchaseOrderRequest) =>
    {
      var purchaseOrderId = Guid.NewGuid();
      var cartGrain = grains.GetGrain<ICartGrain>(ICartGrain.CartGrainId(createPurchaseOrderRequest.CartId));
      var cartResult = await cartGrain.GetCart(new GetCart());
      if (cartResult.Error != null && cartResult.Data == null)
      {
        return Results.BadRequest(cartResult.Error.Type);

      }
      var cart = cartResult.Data!;
      var orderItems = cart.Items.Select(i =>
      {
        var product = new Product(i.Product.Id, i.Product.Name, i.Product.Price, i.Product.PriceExVat, i.Product.Url, i.Product.PrimaryImageUrl);
        return new PurchaseOrderItem(product, i.Quantity);
      }).ToArray();
      var createPurchaseOrderMessage = new CreatePurchaseOrderMessage(purchaseOrderId, createPurchaseOrderRequest.CartId, cart.OperatingChain, cart.SessionId, cart.UserId, orderItems);
      Console.WriteLine("CreatePurchaseOrder: " + System.Text.Json.JsonSerializer.Serialize(createPurchaseOrderMessage));

      var purchaseOrderGrain = grains.GetGrain<IPurchaseOrderGrain>(IPurchaseOrderGrain.PurchaseOrderGrainId(purchaseOrderId));
      var result = await purchaseOrderGrain.CreatePurchaseOrder(createPurchaseOrderMessage);

      return Results.Ok(result);
      // DiagnosticConfig.CreateCartCounter.Add(1, new KeyValuePair<string, object?>("operatingChain", "OCNOELK"));
    });

    routes.MapGet("/{id}", async (IGrainFactory grains, Guid id) =>
    {
    });

    return routes;
  }

  public static WebApplication UseCheckout(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCheckoutEndpoints().WithTags("checkout");
    return app;
  }
}