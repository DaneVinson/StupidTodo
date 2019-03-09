using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.AzureStorage
{
    public class TodoTableRepository : ITodoRepository
    {
        public TodoTableRepository(AzureStorageOptions options)
        {
            Options = options ?? throw new ArgumentNullException();
        }

        #region ITodoRepository

        public Task<IEnumerable<Todo>> AddTodosAsync(IEnumerable<Todo> todos)
        {
            return InsertOrUpdateTodosAsync(todos);
        }

        public async Task<bool> DeleteTodoAsync(string userId, string id)
        {
            var result = await Options.ExecuteWithTableAsync(
                                        nameof(Todo),
                                        TableOperation.Delete(new DynamicTableEntity(userId, id) { ETag = "*" }));
            return result.HttpStatusCode.IsSuccessCode();
        }

        public async Task<IEnumerable<Todo>> GetTodosAsync(string userId)
        {
            var entities = await Options.GetCloudTable(nameof(Todo))
                                        .ExecuteTableQueryAsync(new TableQuery<TodoAdapter>()
                                                                        .Where(TableQuery.GenerateFilterCondition(
                                                                                            "PartitionKey",
                                                                                            QueryComparisons.Equal,
                                                                                            userId)));
            return entities.Select(e => e.OriginalEntity);
        }

        public Task<IEnumerable<Todo>> UpdateTodosAsync(IEnumerable<Todo> todos)
        {
            return InsertOrUpdateTodosAsync(todos);
        }

        #endregion

        public async Task<IEnumerable<Todo>> InsertOrUpdateTodosAsync(IEnumerable<Todo> todos)
        {
            var batchOperation = new TableBatchOperation();
            foreach (var todo in todos)
            {
                batchOperation.InsertOrReplace(new TodoAdapter(todo) { ETag = "*" });
            }
            var results = await Options
                                    .GetCloudTable(nameof(Todo))
                                    .ExecuteBatchAsync(batchOperation);
            return results
                    .Where(r => r.HttpStatusCode.IsSuccessCode())
                    .Select(r => ((TodoAdapter)r.Result)?.OriginalEntity)
                    .ToArray();
        }

        #region Examples of Insert and Replace TableOperation

        public async Task<Todo> AddTodoAsync(Todo todo)
        {
            var result = await Options.ExecuteWithTableAsync(
                                        nameof(Todo),
                                        TableOperation.Insert(new TodoAdapter(todo)));
            if (result.HttpStatusCode.IsSuccessCode()) { return todo; }
            return null;
        }

        public async Task<Todo> UpdateTodoAsync(Todo todo)
        {
            var result = await Options.ExecuteWithTableAsync(nameof(Todo), TableOperation.Replace(new TodoAdapter(todo) { ETag = "*" }));
            if (result.HttpStatusCode.IsSuccessCode()) { return todo; }
            return null;
        }

        #endregion

        private readonly AzureStorageOptions Options;
    }
}
