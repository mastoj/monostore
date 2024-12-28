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
using Microsoft.AspNetCore.Http;
using MonoStore.Cart.Contracts.Requests;

public static class CheckoutEndpoints
{
  public static Meter Meter => new Meter("MonoStore.Checkout.Api");
  public static Counter<long> OperationsCounter => Meter.CreateCounter<long>("operations");

  // public static Counter<long> CartItemsCount => Meter.CreateCounter<long>("cart.items.add");
  // public static Histogram<long> CartValue => GetMeter(apiServiceName).CreateHistogram<long>("cart.value");


  public static string CheckoutGrainId(Guid cartId) => $"cart/{cartId.ToString().ToLower()}";
  public static RouteGroupBuilder MapCheckoutEndpoints(this RouteGroupBuilder routes)
  {
    routes.MapPost("/", async (IGrainFactory grains, CreateCart createCart) =>
    {
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