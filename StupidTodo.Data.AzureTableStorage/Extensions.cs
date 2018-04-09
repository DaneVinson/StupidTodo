using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Data.AzureTableStorage
{
    public static class Extensions
    {
        public static async Task<List<TElement>> ExecuteTableQueryAsync<TElement>(
            this CloudTable cloudTable,
            TableQuery<TElement> tableQuery)
            where TElement : TableEntity, new()
        {
            // Execute a segmentated query (1000 rows per request). This is the only
            // available async query mechanism for Azure Storage Tables (03/2018).
            TableContinuationToken continuationToken = null;
            List<TElement> list = new List<TElement>();
            do
            {
                var tableQueryResult = await cloudTable.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;
                list.AddRange(tableQueryResult.Results);
            } while (continuationToken != null);

            // Return the list.
            return list;
        }

        public static CloudTable GetCloudTable(this AzureTableStorageOptions options, string tableName)
        {
            var account = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={options.AccountName};AccountKey={options.Key}");
            var client = account?.CreateCloudTableClient();
            return client?.GetTableReference(tableName);
        }

        public static TodoTableEntity GetTodoTableEntity(this ITodo todo)
        {
            return new TodoTableEntity()
            {
                Description = todo.Description,
                Done = todo.Done,
                ETag = "*",
                Id = todo.Id,
                PartitionKey = "default_user",
                RowKey = todo.Id
            };
        }
    }
}
