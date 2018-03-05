using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Data.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IProjector<T>
    {
        Task<Result> ProjectAsync(T entity);
        Task<Result> RemoveProjection(T entity);
    }

    public class AzureTableStorageTodoProjector : IProjector<Todo>
    {
        public AzureTableStorageTodoProjector(AzureTableStorageOptions options)
        {
            Options = options;
        }


        public async Task<Result> ProjectAsync(Todo todo)
        {
            var result = new Result();
            var tableResult = await Options.GetCloudTable(AzureTableName)
                                            .ExecuteAsync(TableOperation.InsertOrReplace(todo.GetTodoTableEntity()));
            if (tableResult.HttpStatusCode.IsSuccessCode()) { result.Success = true; }
            else { result.Message = $"InsertOrReplace failed for Todo {todo?.Id}"; }
            return result;
        }

        public async Task<Result> RemoveProjection(Todo todo)
        {
            var result = new Result();
            var tableResult = await Options.GetCloudTable(AzureTableName)
                                            .ExecuteAsync(TableOperation.Delete(todo.GetTodoTableEntity()));
            return result;
        }


        private const string AzureTableName = "stupidtodoview";
        private readonly AzureTableStorageOptions Options;
    }
}
