using System.Net;
using dotenv.net;
using Monostore.ServiceDefaults;
using MonoStore.Cart.Module;
using MonoStore.Product.Api;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
// [assembly: GenerateCodeForDeclaringAssembly(typeof(MonoStore.Cart.Contracts.Cart))]

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up!");

try
{
    DotEnv.Load();

    var config = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddConfiguration(config);

    var serviceName = builder.Configuration["OTEL_RESOURCE_NAME"] ?? "monostore-api";

    var attributes = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split(',').Select(s => s.Split("=")) ?? [];
    var serviceInstanceId = attributes.FirstOrDefault(y => y[0].Contains("service.instance.id"))?[1] ?? throw new Exception("Service instance id not found");
    builder.AddServiceDefaults(c =>
    {
        c.AddService(serviceName, serviceInstanceId: serviceInstanceId);
    }, cm =>
    {
        cm.AddMeter(DiagnosticConfig.GetMeter(serviceName).Name);
    });

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSerilog((services, lc) =>
    {
        lc.ReadFrom.Configuration(config)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.OpenTelemetry(options =>
            {
                //options.Endpoint = "http://monostore-jaeger:4318";
                options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://monostore-jaeger:4317";
                Console.WriteLine($"OTLP Endpoint: {options.Endpoint}");
                var headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];
                foreach (var header in headers)
                {
                    Console.WriteLine($"Header: {header}");
                    var (key, value) = header.Split('=') switch
                    {
                    [string k, string v] => (k, v),
                        var v => throw new Exception($"Invalid header format {v}")
                    };
                    options.Headers.Add(key, value);
                }
                options.ResourceAttributes.Add("service.name", serviceName);
                // options.ResourceAttributes.Add("service.name", serviceName);
                //To remove the duplicate issue, we can use the below code to get the key and value from the configuration
                foreach (var attribute in attributes)
                {
                    options.ResourceAttributes.Add(attribute[0], attribute[1]);
                }
            })
            .WriteTo.Console(new ExpressionTemplate(
                // Include trace and span ids when present.
                "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}",
                theme: TemplateTheme.Code));
    });

    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.AddKeyedAzureTableClient("clustering");
    builder.AddKeyedAzureBlobClient("grainstate");

    builder.Host.UseOrleansClient((ctx, builder) =>
    {
        builder.AddActivityPropagation();
    });

    var app = builder.Build();
    app.UseSerilogRequestLogging();

    #region Endpoints
    app.MapGet("/", () => "Hello World!");
    app.UseCart("cart");
    app.UseProduct("product");
    #endregion

    if (app.Environment.IsDevelopment())
    {
        app.MapScalarApiReference();
    }

    app.MapDefaultEndpoints();
    await app.RunAsync();
    Log.Information("Stopped");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}