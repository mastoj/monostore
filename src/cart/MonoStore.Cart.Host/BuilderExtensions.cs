using Marten;
using Marten.Services;
using MonoStore.Cart.Module;

public static class BuilderExtensions
{
  public static HostApplicationBuilder AddCart(this HostApplicationBuilder builder)
  {
    Action doStuff = () =>
    {

      //var connectionString = $"{builder.Configuration.GetConnectionString("cart")};sslmode=prefer;CommandTimeout=300";
      // Connection string with maximum pool size of 30
      var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
      Console.WriteLine($"Cart ConnectionString: {connectionString}, schema: {Environment.GetEnvironmentVariable("SchemaName")}");
      builder.Services
        .AddSingleton<IEventStore, MartenEventStore>();
      builder.Services.AddMarten(s =>
        {
          var options = new StoreOptions();
          options.Events.MetadataConfig.CorrelationIdEnabled = true;
          options.Events.MetadataConfig.CausationIdEnabled = true;
          // var schemaName = Environment.GetEnvironmentVariable("SchemaName") ?? "public";
          // options.Events.DatabaseSchemaName = schemaName;
          // options.DatabaseSchemaName = schemaName;
          options.DatabaseSchemaName = "monostore";
          options.Connection(connectionString ?? throw new InvalidOperationException());
          options.OpenTelemetry.TrackConnections = TrackLevel.Verbose;
          options.OpenTelemetry.TrackEventCounters();
          return options;
        })
        // .UseNpgsqlDataSource(s => {
        //   s.
        // })
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