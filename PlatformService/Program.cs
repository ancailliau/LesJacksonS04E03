using System;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.SyncDataService.gRPC;
using PlatformService.SyncDataService.HTTP;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Use Postgres database");
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    Console.WriteLine("--> Use in-memory database");
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseInMemoryDatabase("InMem"));   
}

builder.Services.AddControllers();

builder.Services.AddScoped<IPlatformRepository, PlatformEFCoreRepository>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClientService>();
builder.Services.AddSingleton<IMessageBusClient, RabbitMQMessageBusClient>();
builder.Services.AddGrpc();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Setup a HTTP/2 endpoint without TLS.
        options.ListenLocalhost(5052, o => o.Protocols =
            HttpProtocols.Http2);
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<gRPCPlatformService>();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();