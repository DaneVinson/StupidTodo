using Microsoft.Extensions.Configuration;
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
            try
            {
                var configuration = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json")
                                            .AddJsonFile("todos.json")
                                            .Build();

                // Quick way to get a simple setting
                string api1 = configuration.GetSection("todoApiUri")?.Value;

                // Note configuration is case-insensitive
                string api2 = configuration.GetSection("TODOAPIURI")?.Value;

                // A slick way to get even complex data, very resilient, =NOTE= my blog
                string seconds = configuration["apiQuery:frequencySeconds"];

                // Bind to an object
                var options1 = new QueryOptions();
                configuration.GetSection("apiQuery").Bind(options1);

                // From book?
                var options2 = configuration.GetSection("apiQuery").Get<QueryOptions>();

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
                Console.WriteLine();
                Console.WriteLine("...");
                Console.ReadKey();
            }
        }
    }
}
