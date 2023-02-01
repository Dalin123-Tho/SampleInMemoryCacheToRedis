using Microsoft.Extensions.Caching.Memory;

namespace MemoryCacheToRedis.Service
{
    public abstract class MemoryCacheService<T>
    {
        private static IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

        public static void Add(string cacheKey, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            memoryCache.Set(cacheKey, value);
        }
        
        public static T Get(string cacheKey)
        {
            var result = memoryCache.Get(cacheKey);
            return (T)result;
        }
        
        public static void Delete (string cacheKey)
        {
            memoryCache.Remove(cacheKey);  
        }
    }
}
