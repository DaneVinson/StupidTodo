using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StupidTodo.Data.AzureTableStorage;
using StupidTodo.Domain;
using StupidTodo.Domain.EventSource;
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
                Configure();

                //DoEventSourceStuff().GetAwaiter().GetResult();
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

        private static void Configure()
        {
            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json", false, true)
                                        .AddJsonFile("appsettings.development.json", true, true)
                                        .AddEnvironmentVariables()
                                        .Build();
            TableStorageOptions = new AzureTableStorageOptions();
            configuration.GetSection("AzureTableStorage").Bind(TableStorageOptions);
        }

        private static async Task DoEventSourceStuff()
        {
            string todoId = "";
            ICommand command;

            var queueWriter = new CommandQueueWriter(new CommandDispatcher<Todo>(
                                                                new AzureTableStorageEventStore(TableStorageOptions),
                                                                new AzureTableStorageTodoProjector(TableStorageOptions)));

            command = new CreateCommand() { Description = "New thing to do", };
            await queueWriter.WriteAsync(command);

            //command = new UpdateCommand() { Description = "Updated thing", TargetId = todoId };
            //await queueWriter.WriteAsync(command);
            //command = new UpdateCommand() { Description = "Updated thing again", TargetId = todoId };
            //await queueWriter.WriteAsync(command);
            //command = new UpdateCommand() { Done = true, TargetId = todoId };
            //await queueWriter.WriteAsync(command);

            //command = new DeleteCommand() { TargetId = todoId };
            //await queueWriter.WriteAsync(command);
        }

        private static AzureTableStorageOptions TableStorageOptions { get; set; }
    }
}
