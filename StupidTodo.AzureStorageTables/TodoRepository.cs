using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.AzureStorageTables
{
    public class TodoRepository : ITodoRepository
    {
        public TodoRepository(AzureStorageOptions options)
        {
            Options = options ?? throw new ArgumentNullException();
        }


        public async Task<Todo> AddTodoAsync(Todo todo)
        {
            var result = await Options.ExecuteWithTableAsync(
                                        nameof(Todo),
                                        TableOperation.Insert(new TodoAdapter(todo)));
            if (result.HttpStatusCode.IsSuccessCode()) { return todo; }
            return null;
        }

        public async Task<bool> DeleteTodoAsync(string userId, string id)
        {
            var result = await Options.ExecuteWithTableAsync(
                                        nameof(Todo),
                                        TableOperation.Delete(new DynamicTableEntity(userId, id) { ETag = "*" }));
            return result.HttpStatusCode.IsSuccessCode();
        }

        public async Task<IEnumerable<Todo>> GetTodosAsync(string userId, bool done = false)
        {
            var entities = await Options.GetCloudTable(nameof(Todo))
                                        .ExecuteTableQueryAsync(new TableQuery<TodoAdapter>()
                                                                        .Where(TableQuery.GenerateFilterCondition(
                                                                                            "PartitionKey",
                                                                                            QueryComparisons.Equal,
                                                                                            userId)));
            return entities.Select(e => e.OriginalEntity)
                            .Where(t => t.Done == done);
        }

        public async Task<Todo> UpdateTodoAsync(Todo todo)
        {
            var result = await Options.ExecuteWithTableAsync(nameof(Todo), TableOperation.Replace(new TodoAdapter(todo)));
            if (result.HttpStatusCode.IsSuccessCode()) { return todo; }
            return null;
        }


        private readonly AzureStorageOptions Options;
    }
}
