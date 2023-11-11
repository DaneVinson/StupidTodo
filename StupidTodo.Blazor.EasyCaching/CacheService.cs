using EasyCaching.Core;
using EasyCaching.Core.Configurations;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.EasyCaching;

public class CacheService
{
    private readonly CachingOptions _options;
    private readonly IEasyCachingProvider _provider;

    public CacheService(IEasyCachingProvider provider, CachingOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cache = await _provider.GetAsync<T>(key);
        if (cache.HasValue)
        {
            return cache.Value;
        }

        return default;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await _provider.SetAsync(key, value, _options.Lifetime);
    }
}