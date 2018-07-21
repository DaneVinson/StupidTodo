using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StupidTodo.Data.AzureTableStorage;
using StupidTodo.Domain;
using StupidTodo.Domain.EventSource2;
using StupidTodo.Domain2.EventSource;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StupidTodo.AdminConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DoStuffAsync().GetAwaiter().GetResult();
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

        private static async Task DoStuffAsync()
        {
            using (var serviceProvider = GetServiceProvider())
            {
                var command = new UpdateTodoCommand(Guid.Parse("298abe23-4d63-4f7f-889c-5d552e50d9b0"), "Updated", true);
                var messenger = serviceProvider.GetService<IMessenger<ICommand>>();
                var result = await messenger.SendMessageAsync(command);
            }
        }
        private static ServiceProvider GetServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json", false, true)
                                        .AddJsonFile("appsettings.development.json", true, true)
                                        .AddEnvironmentVariables()
                                        .Build();

            return new ServiceCollection()
                        .AddSingleton(configuration.NewObjectFromSection<AzureTableStorageOptions>("AzureTableStorage"))
                        .AddTransient<IProjector<ITodo, bool>, AzureTableStorageTodoProjector>()
                        .AddTransient<IMessenger<ICommand>, MessengerDispatcher<ICommand>>()
                        .AddTransient<IMessenger<IEvent>, MessengerDispatcher<IEvent>>()
                        .AddTransient<IDispatcher<ICommand>, CommandDispatcher>()
                        .AddTransient<IDispatcher<IEvent>, CommandEventsDispatcher>()
                        .AddTransient<ICommandHandler, TodoCommandHandler>()
                        .AddTransient<IAggregate<ICommand, IEvent, ITodoState>, TodoEntity>()
                        .AddTransient<IEventStore, AzureTableStorageEventStore>()
                        .BuildServiceProvider();
        }

        private static AzureTableStorageOptions TableStorageOptions { get; set; }
    }
}
