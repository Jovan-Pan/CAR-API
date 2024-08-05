using Contracts.Infrastructure;
using Infrastructure;

namespace WebApi.DependecyInjection;

public static class MemoryCacheExtension
{
    public static void AddMemCacheConfigurations(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheManager, CacheManager>();
    }
}
