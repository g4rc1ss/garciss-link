using System.Text.RegularExpressions;
using GarcissLink.Services;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();
builder.Services.AddHostedService<WebAlwaysUpOnRender>();
builder.Services.AddDistributedMemoryCache();

// builder.Services.AddStackExchangeRedisCache(redis =>
// {
//     redis.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
//     // redis.InstanceName = "GarcissLink";
// });
builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(
    (x) =>
        ConnectionMultiplexer.Connect(
            builder.Configuration.GetConnectionString("RedisConnection")
                ?? throw new InvalidOperationException("RedisConnection not found")
        )
);

WebApplication app = builder.Build();

app.MapGet("/health", () => "Healthy");

// app.MapGet("/", async (IConnectionMultiplexer connectionMultiplexer) =>
// {
//     var keys = connectionMultiplexer
//         .GetServers()[0]
//         .KeysAsync(0);
//
//     var keysList = new List<string>();
//     await foreach (var item in keys)
//     {
//         keysList.Add(item);
//     }
//
//     return keysList;
// });

app.MapGet(
    "/{path}",
    async (string path, IDistributedCache cache) =>
    {
        string? result = await cache.GetStringAsync(path);

        return string.IsNullOrEmpty(result)
            ? Results.BadRequest("No hay registros guardados sobre este enlace")
            : Results.Redirect(result);
    }
);

app.MapPost(
    "/",
    async (Link link, IDistributedCache cache) =>
    {
        if (!MyRegex().IsMatch(link.Url))
        {
            return Results.BadRequest("Tienes que insertar una URL completa");
        }

        await cache.SetStringAsync(link.ShortPath, link.Url);

        return Results.Ok();
    }
);

app.UseSwagger();
app.UseSwaggerUI();
await app.RunAsync();

record Link(string Url, string ShortPath);

partial class Program
{
    [GeneratedRegex(
        @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"
    )]
    private static partial Regex MyRegex();
}
