using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StupidTodo.Domain;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StupidTodo.ClientConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServiceProvider serviceProvider = null;
            try
            {
                serviceProvider = NewServiceProvider();
                var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

                // Quick way to get a simple setting
                string api1 = configuration.GetSection("todoApiUri")?.Value;

                // Note configuration is case-insensitive
                string api2 = configuration.GetSection("TODOAPIURI")?.Value;

                // A slick way to get even complex data, very resilient, =NOTE= my blog
                string seconds = configuration["apiQuery:frequencySeconds"];

                // Bind to an object
                var options1 = new QueryOptions();
                configuration.GetSection("apiQuery").Bind(options1);

                // A more elegant method
                var options2 = configuration.GetSection("apiQuery").Get<QueryOptions>();

                // =NOTE= from the book
                var options3 = serviceProvider.GetService<IOptions<QueryOptions>>()?.Value;

                // Note additional properties in config are ignored and additional properties on the binding class are default.
                // Again very resiliant.

                // Get can be used to handle arrays of complex objects
                var todos = configuration.GetSection("todos").Get<Todo[]>();

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} - {1}", ex.GetType(), ex.Message);
                Console.WriteLine(ex.StackTrace ?? String.Empty);
            }
            finally
            {
                if (serviceProvider != null) { serviceProvider.Dispose(); }
                Console.WriteLine();
                Console.WriteLine("...");
                Console.ReadKey();
            }
        }

        private static ServiceProvider NewServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json")
                                        .AddJsonFile("todos.json")
                                        .Build();

            return new ServiceCollection()
                            .AddSingleton(configuration)
                            // =NOTE= from the book
                            .Configure<QueryOptions>(configuration.GetSection("apiQuery"))
                            .BuildServiceProvider();
        }
    }
}
