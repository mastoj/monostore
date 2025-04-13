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
using Marten;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MonoStore.Checkout.Domain;
using Monostore.Orleans.Types;

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
    }).Produces<GrainResult<PurchaseOrderData, CheckoutError>>();

    routes.MapGet("/", async (ICheckoutStore checkoutStore, string operatingChain, string? userId, string? sessionId, string? sku, string? cartId, string? query) =>
    {
      await using var session = checkoutStore.LightweightSession();
      var querySession = session.Query<PurchaseOrder>().Where(c => c.OperatingChain == operatingChain);
      if (userId != null)
      {
        querySession = querySession.Where(c => c.UserId == userId);
      }
      if (sessionId != null)
      {
        querySession = querySession.Where(c => c.SessionId == sessionId);
      }
      if (sku != null)
      {
        querySession = querySession.Where(c => c.Items.Any(i => i.Product.Id == sku));
      }
      if (cartId != null)
      {
        querySession = querySession.Where(c => c.CartId == Guid.Parse(cartId));
      }

      var carts = await querySession.ToListAsync();
      return Results.Ok(carts);
    }).Produces<List<PurchaseOrder>>();

    routes.MapGet("/{id}", async (IGrainFactory grainFactory, Guid id) =>
    {
      var purchaseOrderGrain = grainFactory.GetGrain<IPurchaseOrderGrain>(IPurchaseOrderGrain.PurchaseOrderGrainId(id));
      var result = await purchaseOrderGrain.GetPurchaseOrder(new GetPurchaseOrder());
      return Results.Ok(result);
    }).Produces<GrainResult<PurchaseOrderData, CheckoutError>>();

    routes.MapGet("/{id}/changes", async (ICheckoutStore checkoutStore, Guid id) =>
    {
      await using var session = checkoutStore.LightweightSession();
      var changes = await session.Events.FetchStreamAsync(id);
      var result = changes?.Select(c => new Change(c.EventTypeName, c.Timestamp, c.Version, c.Data)).ToList();
      return Results.Ok(result);
    }).Produces<List<Change>>();

    return routes;
  }

  public static T AddCheckout<T>(this T builder) where T : IHostApplicationBuilder
  {
    var connectionStringName = "monostorepg";
    var databaseSchemaName = "checkout";
    var connectionString = $"{builder.Configuration.GetConnectionString(connectionStringName)};sslmode=prefer;CommandTimeout=60;Pooling=true;Maximum Pool Size=20;Minimum Pool Size=5;Connection Lifetime=30;Connection Idle Lifetime=10;Connection Pruning Interval=3";

    builder.Services.AddMartenStore<ICheckoutStore>(s =>
    {
      var options = new StoreOptions();
      options.Events.MetadataConfig.CorrelationIdEnabled = true;
      options.Events.MetadataConfig.CausationIdEnabled = true;
      options.DatabaseSchemaName = databaseSchemaName;
      options.Connection(connectionString ?? throw new InvalidOperationException());
      options.OpenTelemetry.TrackEventCounters();
      options.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.None;
      return options;
    });

    return builder;
  }

  public static WebApplication UseCheckout(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCheckoutEndpoints().WithTags("checkout");
    return app;
  }
}

public interface ICheckoutStore : IDocumentStore { }