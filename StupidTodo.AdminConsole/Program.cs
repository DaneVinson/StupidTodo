using Newtonsoft.Json;
using StupidTodo.Domain.EventSource;
using System;
using System.Threading.Tasks;

namespace StupidTodo.AdminConsole
{
    class Program
    {
        private static readonly string AccountName = "";
        private static readonly string Key = "";

        static void Main(string[] args)
        {
            try
            {
                CreateTodoAsync("Another todo").GetAwaiter().GetResult();
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

        private static async Task CreateTodoAsync(string description)
        {
            var options = new AzureTableStorageEventStoreOptions()
            {
                AccountName = AccountName,
                Key = Key
            };
            var schema = new CreateEventSchema()
            {
                Description = description,
                Id = Guid.NewGuid().ToString()
            };
            var record = new EventRecord()
            {
                EventData = JsonConvert.SerializeObject(schema),
                EventType = typeof(Created).ToString(),
                Id = Guid.NewGuid().ToString(),
                OwnerId = schema.Id
            };

            var eventStore = new AzureTableStorageEventStore(options);
            var todoId = await eventStore.AddEventRecordAsync(record);
            Console.WriteLine($"New Todo Id: {todoId}");
        }
    }
}
