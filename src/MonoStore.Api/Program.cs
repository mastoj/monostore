using dotenv.net;
using Monostore.ServiceDefaults;
using MonoStore.Cart.Module;
using MonoStore.Product.Module;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
[assembly: GenerateCodeForDeclaringAssembly(typeof(MonoStore.Cart.Contracts.Cart))]

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

    var serviceInstanceId = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split('=') switch
    {
    [string k, string v] => v,
        _ => throw new Exception($"Invalid header format {builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]}")
    };

    builder.AddServiceDefaults(c =>
    {
        c.AddService(serviceName, serviceInstanceId: serviceInstanceId);
    }, cm =>
    {
        cm.AddMeter(DiagnosticConfig.meter.Name);
    });

    builder.Services.AddSerilog((services, lc) =>
    {
        lc.ReadFrom.Configuration(config)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
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
                var (otelResourceAttribute, otelResourceAttributeValue) = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split('=') switch
                {
                [string k, string v] => (k, v),
                    _ => throw new Exception($"Invalid header format {builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]}")
                };
                Console.WriteLine($"Resource Attribute: {otelResourceAttribute}, Resource Attribute Value: {otelResourceAttributeValue}");
                options.ResourceAttributes.Add(otelResourceAttribute, otelResourceAttributeValue);
            })
            .WriteTo.Console(new ExpressionTemplate(
                // Include trace and span ids when present.
                "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}",
                theme: TemplateTheme.Code));
    });

    // Add services to the container.
    // builder.Services.AddRazorPages();
    builder.Host.UseOrleans(static siloBuilder =>
    {
        siloBuilder
            .UseLocalhostClustering()
            .AddMemoryGrainStorage("carts")
            .AddActivityPropagation();
    });

    builder.AddNpgsqlDataSource("cart");

    #region Domains
    builder.AddCart();
    builder.AddProduct();
    #endregion

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    #region Endpoints
    app.UseCart("cart");
    #endregion

    app.MapDefaultEndpoints();

    // app.MapGroup("cart").MapCartEndpoints();

    // // Configure the HTTP request pipeline.
    // // if (!app.Environment.IsDevelopment())
    // // {
    // //     app.UseExceptionHandler("/Error");
    // //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // //     app.UseHsts();
    // // }

    // // app.UseHttpsRedirection();
    // // // app.UseStaticFiles();

    // // app.UseRouting();

    //app.UseAuthorization();

    // // // app.MapRazorPages();

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