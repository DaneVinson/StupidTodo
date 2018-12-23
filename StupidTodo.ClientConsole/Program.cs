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
                    var beer = serviceProvider.GetService<IOptions<TheBeer>>()?.Value;
                    var todos = serviceProvider.GetService<IOptions<List<Todo>>>()?.Value;
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
                        .Configure<TheBeer>(configuration.GetSection("beerOfTheNow"))
                        .Configure<List<Todo>>(configuration.GetSection("todos"))
                        .BuildServiceProvider();
        }
    }
}
