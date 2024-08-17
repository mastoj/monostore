using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MonoStore.Cart.Module;

public class DummyEventStore : IEventStore
{
  private static Dictionary<Guid, List<object>> _streams = new();
  public Task<T> AppendToStream<T>(Guid id, T @event, CancellationToken ct)
  {
    if (!_streams.ContainsKey(id))
    {
      _streams[id] = new List<object>();
    }

    _streams[id].Add(@event);
    return Task.FromResult(@event);
  }

  public Task<T> CreateStream<T>(Guid id, T @event, CancellationToken ct)
  {
    if (!_streams.ContainsKey(id))
    {
      _streams[id] = new List<object>();
    }

    _streams[id].Add(@event);
    return Task.FromResult(@event);
  }
}

public static class ApiExtensions
{
  public static WebApplicationBuilder AddCart(this WebApplicationBuilder builder)
  {
    builder.Services.AddTransient<IEventStore, DummyEventStore>();
    return builder;
  }


  public static WebApplication UseCart(this WebApplication app, string groupPath)
  {
    app.MapGroup(groupPath).MapCartEndpoints();
    return app;
  }
}
