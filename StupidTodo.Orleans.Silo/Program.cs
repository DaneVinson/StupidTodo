using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using StupidTodo.AzureStorage;
using StupidTodo.Domain;
using StupidTodo.Orleans.Grains;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StupidTodo.Orleans.Silo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().GetAwaiter().GetResult();
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press any key terminate...");
                Console.ReadKey();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} - {1}", ex.GetType(), ex.Message);
                Console.WriteLine(ex.StackTrace ?? String.Empty);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder().UseLocalhostClustering()
                                                .Configure<ClusterOptions>(options =>
                                                {
                                                    options.ClusterId = "development";
                                                    options.ServiceId = "StupidTodo";
                                                })
                                                .ConfigureServices(services =>
                                                {
                                                    services.AddSingleton(Configuration.GetSection("AzureStorage").Get<AzureStorageOptions>())
                                                            .AddTransient<ITodoRepository, TodoTableRepository>();
                                                })
                                                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(UserTodosGrain).Assembly).WithReferences())
                                                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json")
                                    .AddJsonFile("appsettings.Development.json")
                                    .Build();
        }

        private static readonly IConfigurationRoot Configuration;
    }
}
