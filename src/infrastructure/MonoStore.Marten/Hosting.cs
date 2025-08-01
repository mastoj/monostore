using Marten;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonoStore.Marten;

namespace Microsoft.Extensions.Hosting;

public static class Hosting
{
  public static IHostApplicationBuilder UseMartenEventStore(this IHostApplicationBuilder builder, string connectionStringName, string databaseSchemaName, Func<StoreOptions, StoreOptions>? storeOptionsConfig = null)
  {
    Action doStuff = () =>
    {

      var connectionString = $"{builder.Configuration.GetConnectionString(connectionStringName)};sslmode=prefer;CommandTimeout=300;Pooling=true;MinPoolSize=1;MaxPoolSize=100";
      builder.Services
        .AddSingleton<IEventStore, MartenEventStore>();
      builder.Services.AddMarten(s =>
        {
          var options = new StoreOptions();
          options.Events.MetadataConfig.CorrelationIdEnabled = true;
          options.Events.MetadataConfig.CausationIdEnabled = true;
          options.DatabaseSchemaName = databaseSchemaName;
          options.AutoCreateSchemaObjects = JasperFx.AutoCreate.None;
          options.Connection(connectionString ?? throw new InvalidOperationException());
          // options.OpenTelemetry.TrackConnections = TrackLevel.Verbose;
          options.OpenTelemetry.TrackEventCounters();
          if (storeOptionsConfig != null)
          {
            options = storeOptionsConfig(options);
          }
          return options;
        })
        .UseLightweightSessions()
        .ApplyAllDatabaseChangesOnStartup();
    };
    var attempts = 0;
    var maxAttempts = 3;
    while (attempts < maxAttempts)
    {
      try
      {
        doStuff();
        break;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Attempt {attempts + 1} failed: {ex.Message}");
        attempts++;
        // Sleep 1 second
        Thread.Sleep(1000);
        if (attempts == maxAttempts)
        {
          throw;
        }
      }
    }
    return builder;
  }
}
