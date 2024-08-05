namespace Contracts.Infrastructure;

public interface ICacheManager
{
    public T GetCache<T>(string cacheKey);
    public void SetCache<T>(string cacheKey, T value);
}
