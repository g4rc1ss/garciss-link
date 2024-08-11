using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(redis =>
{
    redis.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    // redis.InstanceName = "GarcissLink";
});
builder.Services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>((x) =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection"));
});

var app = builder.Build();


app.MapGet("/health", () => "Healthy");

app.MapGet("/", async (IConnectionMultiplexer connectionMultiplexer) =>
{
    var keys = connectionMultiplexer
        .GetServers()[0]
        .KeysAsync(0);

    var keysList = new List<string>();
    await foreach (var item in keys)
    {
        keysList.Add(item);
    }

    return keysList;
});

app.MapGet("/{path}", async (string path, IDistributedCache cache) =>
{
    var result = await cache.GetStringAsync(path);

    return string.IsNullOrEmpty(result)
        ? Results.BadRequest("No hay registros guardados sobre este enlace")
        : Results.Redirect(result);
});

app.MapPost("/", async (Link link, IDistributedCache cache) =>
{
    if (!Regex.IsMatch(link.Url, @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"))
    {
        return Results.BadRequest("Tienes que insertar una URL completa");
    }

    await cache.SetStringAsync(link.ShortPath, link.Url);

    return Results.Ok();
});



app.UseSwagger();
app.UseSwaggerUI();
await app.RunAsync();


record Link(string Url, string ShortPath);