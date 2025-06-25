using GarcissLink.AspireHost.Resources;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource> redis = builder.AddRedisCache();

builder.AddProject<Projects.GarcissLink>("GarcissLink").WithRedisCache(redis);

await builder.Build().RunAsync();
