using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.LazyCache
{
    public static class Extensions
    {
        public static IServiceCollection AddCachingLazyCache(this IServiceCollection services)
        {
            services.AddSingleton<IAppCache>(new CachingService());

            return services;
        }
    }
}
