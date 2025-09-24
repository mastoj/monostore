using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MonoStore.MCP;

public static class MCPExtensions
{
  public static T AddMCP<T>(this T builder) where T : IHostApplicationBuilder
  {
    builder.Services
      .AddMcpServer()
      .WithHttpTransport()
      .WithToolsFromAssembly();
    return builder;
  }

  public static WebApplication UseMCP(this WebApplication app, string path)
  {
    app.MapMcp(path);
    return app;
  }
}
