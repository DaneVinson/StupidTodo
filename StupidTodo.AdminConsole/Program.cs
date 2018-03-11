using Newtonsoft.Json;
using StupidTodo.Data.AzureTableStorage;
using StupidTodo.Domain;
using StupidTodo.Domain.EventSource;
using System;
using System.Threading.Tasks;

namespace StupidTodo.AdminConsole
{
    class Program
    {
		private static readonly AzureTableStorageOptions AzureTableStorageOptions = new AzureTableStorageOptions()
        {
            AccountName = "danestorage",
            Key = "FllBAnlnhTpVN+FBd1j4nq4MbJT75Fno2zEVThSXb0k6GWeXrN8/KHJ0NeheHC+LTTNr19T0+/ybc7I0JKkjFw=="
        };
        //private static readonly AzureTableStorageOptions AzureTableStorageOptions = new AzureTableStorageOptions()
        //{
        //    AccountName = "{account}",
        //    Key = "{key}"
        //};

        static void Main(string[] args)
        {
            try
            {
                DoEventSourceStuff().GetAwaiter().GetResult();
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

        private static async Task DoEventSourceStuff()
        {
            string todoId = "";
            ICommand command;

            var queueWriter = new CommandQueueWriter(new CommandDispatcher<Todo>(
                                                                new AzureTableStorageEventStore(AzureTableStorageOptions),
                                                                new AzureTableStorageTodoProjector(AzureTableStorageOptions)));

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
    }
}
