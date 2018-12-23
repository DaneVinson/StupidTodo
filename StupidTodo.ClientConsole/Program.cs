using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StupidTodo.ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var serviceProvider = NewServiceProvider())
                {
                    var configuration = serviceProvider.GetRequiredService<IConfigurationRoot>();

                    // Quick way to get a simple setting
                    string favorite = configuration.GetSection("favoriteBeerStyle")?.Value;

                    // Note configuration is case-insensitive
                    favorite = configuration.GetSection("FAVORITEBEERSTYLE")?.Value;
                    favorite = configuration["favoriteBeerStyle"];

                    // A slick way to get even complex data, very resilient, =NOTE= my blog
                    string current = configuration["beerOfTheNow:name"];
                    string currentBreweryName = configuration["beerOfTheNow:brewery:name"];
                    string badName = configuration["beerOfTheNow:not_a_name"];

                    // Bind to an object
                    var beer = new TheBeer();
                    configuration.GetSection("beerOfTheNow").Bind(beer);

                    // A more elegant method
                    var beer2 = configuration.GetSection("beerOfTheNow").Get<TheBeer>();

                    // =NOTE= from the book
                    var beer3 = serviceProvider.GetService<IOptions<TheBeer>>()?.Value;

                    // Note additional properties in config are ignored and additional properties on the binding class are default.
                    // Again very resiliant.

                    // Loading collections from config
                    var todos = configuration.GetSection("todos").Get<Todo[]>();
                    var todos2 = serviceProvider.GetService<IOptions<List<Todo>>>()?.Value;
                }
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
                            .Configure<TheBeer>(configuration.GetSection("beerOfTheNow"))
                            .Configure<List<Todo>>(configuration.GetSection("todos"))
                            .BuildServiceProvider();
        }
    }
}
