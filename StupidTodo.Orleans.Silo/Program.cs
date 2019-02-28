using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using StupidTodo.Orleans.Grains;
using System;
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
                                                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(UserTodosGrain).Assembly).WithReferences())
                                                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
