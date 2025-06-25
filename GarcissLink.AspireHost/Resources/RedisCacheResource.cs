namespace GarcissLink.AspireHost.Resources;

public static class RedisCacheResource
{
    internal static IResourceBuilder<RedisResource> AddRedisCache(
        this IDistributedApplicationBuilder builder
    )
    {
        return builder.AddRedis("Cache").WithRedisCommander().WithRedisInsight();
    }

    internal static IResourceBuilder<T> WithRedisCache<T>(
        this IResourceBuilder<T> builder,
        IResourceBuilder<RedisResource> redis
    )
        where T : IResourceWithWaitSupport, IResourceWithEnvironment
    {
        return builder.WithReference(redis).WaitFor(redis);
    }
}
