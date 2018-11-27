using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;

namespace DotNetCoreDashboardArch.Services
{

    public interface ICacheProvider
    {
        void AddItem<T>(string cacheKey, DateTime time, T value);
        bool TryGetItem<T>(string cacheKey, DateTime time, out T value);
    }
    

    public class CacheProvider : ICacheProvider
    {
        private readonly MemoryCache _cache;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public CacheProvider(ILoggerFactory loggerFactory)
        {
            MemoryCacheOptions cacheOptions = new MemoryCacheOptions();
            _cache = new MemoryCache(cacheOptions);
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("CacheProvider");
        }


        public void AddItem<T>(string cacheKey, DateTime time, T value)
        {
            if (time == DateTime.MinValue)
            {
                _logger.LogWarning("Cache dependency has failed. Response data is no longer being cached.");
                return;
            }
            string ctsKey = String.Format("{0}_CTS", cacheKey);
            string timeKey = String.Format("{0}_Time", cacheKey);
            CancellationTokenSource cts = new CancellationTokenSource();
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(cts.Token))
                .SetAbsoluteExpiration(TimeSpan.FromHours(6));
            _cache.Set<T>(cacheKey, value, options);
            _cache.Set<DateTime>(timeKey, time, options);
            _cache.Set<CancellationTokenSource>(ctsKey, cts, options);
        }


        public bool TryGetItem<T>(string cacheKey, DateTime lastUpdate, out T value)
        {
            if (lastUpdate == DateTime.MinValue)
            {
                value = default(T);
                return false;
            }
            string timeKey = String.Format("{0}_Time", cacheKey);
            DateTime cacheTimestamp;
            bool timeExists = _cache.TryGetValue(timeKey, out cacheTimestamp);
            if (timeExists && (cacheTimestamp < lastUpdate))
            {
                RemoveItems(cacheKey);
            }
            bool exists = _cache.TryGetValue(cacheKey, out T _value);
            value = _value;
            return exists;
        }

        private void RemoveItems(string cacheKey)
        {
            string ctsKey = String.Format("{0}_CTS", cacheKey);
            _cache.Get<CancellationTokenSource>(ctsKey).Cancel();
        }
    }
}
