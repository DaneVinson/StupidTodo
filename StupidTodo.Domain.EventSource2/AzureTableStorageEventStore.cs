using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Data.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public class AzureTableStorageEventStore : IEventStore
    {
        public AzureTableStorageEventStore(AzureTableStorageOptions options)
        {
            Options = options;
        }


        public async Task<Guid> AddEventRecordAsync(IEventRecord eventRecord)
        {
            var result = await Options.GetCloudTable(AzureTableName).ExecuteAsync(TableOperation.Insert(eventRecord.GetEventRecordTableEntity()));
            if (result.HttpStatusCode.IsSuccessCode()) { return eventRecord.Id; }
            else { return Guid.Empty; }
        }

        public async Task<bool> AddEventRecordsAsync(IEnumerable<IEventRecord> eventRecords)
        {
            var operation = new TableBatchOperation();
            foreach(var record in eventRecords)
            {
                operation.Add(TableOperation.Insert(record.GetEventRecordTableEntity()));
            }
            var result = await Options.GetCloudTable(AzureTableName).ExecuteBatchAsync(operation);

            // TODO: Acting as if the the batch execute is atomic which it is not. Address the situation where only "some" events persisting successfully.
            return result.Any(r => r.HttpStatusCode.IsSuccessCode());
        }

        public async Task<IEnumerable<IEventRecord>> GetEventRecordsAsync(Guid entityId)
        {
            var records = await Options.GetCloudTable(AzureTableName)
                                        .ExecuteTableQueryAsync(new TableQuery<EventRecordTableEntity>()
                                                                        .Where(TableQuery.GenerateFilterCondition(
                                                                                            "PartitionKey",
                                                                                            QueryComparisons.Equal,
                                                                                            entityId.ToString())));
            return records.OrderBy(r => r.Timestamp);
        }


        private const string AzureTableName = "stupidtodoevents";
        private readonly AzureTableStorageOptions Options;
    }
}
