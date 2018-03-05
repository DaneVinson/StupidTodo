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
            AccountName = "{account}",
            Key = "{key}"
        };

        static void Main(string[] args)
        {
            try
            {
                var todoId = CreateTodoAsync("Another todo").GetAwaiter().GetResult();
                UpdateDescription(todoId, "Updated todo").GetAwaiter().GetResult();
                UpdateDescription(todoId, "Updated todo again").GetAwaiter().GetResult();
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


        private static async Task<string> CreateTodoAsync(string description)
        {
            var schema = new CreateEventSchema()
            {
                Description = description,
                Id = Guid.NewGuid().ToString()
            };
            var result = await NewTodoAggregate().AddEvent(schema.Id, typeof(Created), JsonConvert.SerializeObject(schema));
            Console.WriteLine($"New Todo: {result}");

            return schema.Id;
        }

        private static async Task UpdateDescription(string todoId, string description)
        {
            var schema = new UpdateDescriptionEventSchema() { Description = description };
            var result = await NewTodoAggregate().AddEvent(todoId, typeof(DescriptionUpdated), JsonConvert.SerializeObject(schema));
            Console.WriteLine($"Updated description: {result}");
        }

        private static TodoAggregate NewTodoAggregate()
        {
            return new TodoAggregate(
                        new AzureTableStorageEventStore(AzureTableStorageOptions),
                        new AzureTableStorageTodoProjector(AzureTableStorageOptions));
        }
    }
}
