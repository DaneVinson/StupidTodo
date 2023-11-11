using EasyCaching;
using EasyCaching.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.EasyCaching
{
    public static class Extensions
    {
        public static IServiceCollection AddCachingEasyCaching(this IServiceCollection services)
        {
            services.AddEasyCaching(options =>
            {
                options.UseInMemory("ClientCache");

                // use memory cache with your own configuration
                //options.UseInMemory(config =>
                //{
                //    config.DBConfig = new InMemoryCachingOptions
                //    {
                //        // scan time, default value is 60s
                //        ExpirationScanFrequency = 60,
                //        // total count of cache items, default value is 10000
                //        SizeLimit = 100,

                //        // enable deep clone when reading object from cache or not, default value is true.
                //        EnableReadDeepClone = true,
                //        // enable deep clone when writing object to cache or not, default value is false.
                //        EnableWriteDeepClone = false,
                //    };
                //    // the max random second will be added to cache's expiration, default value is 120
                //    config.MaxRdSecond = 120;
                //    // whether enable logging, default is false
                //    config.EnableLogging = false;
                //    // mutex key's alive time(ms), default is 5000
                //    config.LockMs = 5000;
                //    // when mutex key alive, it will sleep some time, default is 300
                //    config.SleepMs = 300;
                //}, 
                //"ClientCache");
            });

            return services;
        }
    }
}
