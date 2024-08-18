var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MonoStore_Api>("api").WithExternalHttpEndpoints();

builder.Build().Run();
