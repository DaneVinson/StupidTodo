using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Data.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IProjector<T>
    {
        Task<Result> PersistProjectionAsync(T entity);
        Task<Result> RemoveProjectionAsync(T entity);
        Task<Result<IEnumerable<T>>> ViewAsync(bool done);
    }


    public class AzureTableStorageTodoProjector : IProjector<Todo>
    {
        public AzureTableStorageTodoProjector(AzureTableStorageOptions options)
        {
            Options = options;
        }


        public async Task<Result> PersistProjectionAsync(Todo todo)
        {
            var result = new Result();
            var tableResult = await Options.GetCloudTable(AzureTableName)
                                            .ExecuteAsync(TableOperation.InsertOrReplace(todo.GetTodoTableEntity()));
            if (tableResult.HttpStatusCode.IsSuccessCode()) { result.Success = true; }
            else { result.Message = $"InsertOrReplace failed for Todo {todo?.Id}"; }
            return result;
        }

        public async Task<Result> RemoveProjectionAsync(Todo todo)
        {
            var result = new Result();
            var tableResult = await Options.GetCloudTable(AzureTableName)
                                            .ExecuteAsync(TableOperation.Delete(todo.GetTodoTableEntity()));
            return result;
        }

        public async Task<Result<IEnumerable<Todo>>> ViewAsync(bool done)
        {
            var result = new Result<IEnumerable<Todo>>();

            var records = await Options.GetCloudTable(AzureTableName)
                                        .ExecuteTableQueryAsync(new TableQuery<TodoTableEntity>()
                                                                        .Where(TableQuery.GenerateFilterCondition(
                                                                                            "PartitionKey",
                                                                                            QueryComparisons.Equal,
                                                                                            "default_user")));
            result.Value = records.Where(t => t.Done == done)
                                    .OrderBy(t => t.Timestamp)
                                    .Select(t => t.GetTodo())
                                    .ToList();
            return result;
        }

        private const string AzureTableName = "stupidtodoview";
        private readonly AzureTableStorageOptions Options;
    }
}
