using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public static class Extensions
    {
        internal static async Task<List<TElement>> ExecuteTableQueryAsync<TElement>(
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

        public static EventTableRecord GetEventTableRecord(this IEventRecord eventRecord)
        {
            var now = DateTime.UtcNow;
            return new EventTableRecord()
            {
                EventData = eventRecord.EventData,
                EventType = eventRecord.EventType,
                Id = eventRecord.Id,
                OwnerId = eventRecord.OwnerId,
                PartitionKey = eventRecord.OwnerId,
                RowKey = eventRecord?.Id ?? Guid.NewGuid().ToString(),
            };
        }

        public static bool IsSuccessCode(this HttpStatusCode code)
        {
            return (int)code > 199 && (int)code < 300;
        }

        public static bool IsSuccessCode(this int code)
        {
            return code > 199 && code < 300;
        }
    }
}
