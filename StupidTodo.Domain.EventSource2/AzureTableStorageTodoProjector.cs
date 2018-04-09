using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Data.AzureTableStorage;
using StupidTodo.Domain;
using StupidTodo.Domain.EventSource2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain2.EventSource
{
    public class AzureTableStorageTodoProjector : IProjector<ITodo, bool>
    {
        public AzureTableStorageTodoProjector(AzureTableStorageOptions options)
        {
            Options = options;
        }


        public async Task<Result> PersistProjectionAsync(ITodo todo)
        {
            var result = new Result();
            var tableResult = await Options.GetCloudTable(AzureTableName)
                                            .ExecuteAsync(TableOperation.InsertOrReplace(todo.GetTodoTableEntity()));
            if (tableResult.HttpStatusCode.IsSuccessCode()) { result.Success = true; }
            else { result.Message = $"InsertOrReplace failed for Todo {todo?.Id}"; }
            return result;
        }

        public async Task<Result> RemoveProjectionAsync(ITodo todo)
        {
            var result = new Result();
            var tableResult = await Options.GetCloudTable(AzureTableName)
                                            .ExecuteAsync(TableOperation.Delete(todo.GetTodoTableEntity()));
            result.Success = tableResult.HttpStatusCode.IsSuccessCode();
            return result;
        }

        public async Task<Result<IEnumerable<ITodo>>> ViewProjectionAsync(bool done)
        {
            var result = new Result<IEnumerable<ITodo>>();

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
            result.Success = true;
            return result;
        }


        private const string AzureTableName = "stupidtodoview";
        private readonly AzureTableStorageOptions Options;
    }
}
