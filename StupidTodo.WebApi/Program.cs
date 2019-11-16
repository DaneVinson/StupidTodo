using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StupidTodo.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    // Azure App Configuration
                    builder.ConfigureAppConfiguration((context, configBuilder) =>
                    {
                        var configuration = configBuilder.Build();
                        configBuilder.AddAzureAppConfiguration(options =>
                        {
                            options.Connect(configuration["AzureAppConfiguration:ConnectionString"])
                                    .Use(KeyFilter.Any, LabelFilter.Null)
                                    .Use(KeyFilter.Any, configuration["AzureAppConfiguration:Instance"]);
                        });
                    })
                    .UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}
