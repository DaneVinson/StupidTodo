using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using StupidTodo.Data.AzureTableStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IEventStore
    {
        Task<string> AddEventRecordAsync(IEventRecord eventRecord);
        Task<IEnumerable<IEventRecord>> GetEventRecordsAsync(string ownerId);
    }


    public class AzureTableStorageEventStore : IEventStore
    {
        public AzureTableStorageEventStore(AzureTableStorageOptions options)
        {
            Options = options;
        }


        public async Task<string> AddEventRecordAsync(IEventRecord eventRecord)
        {
            var result = await Options.GetCloudTable(AzureTableName).ExecuteAsync(TableOperation.Insert(eventRecord.GetEventRecordTableEntity()));
            if (result.HttpStatusCode.IsSuccessCode()) { return eventRecord.Id; }
            else { return null; }
        }

        public async Task<IEnumerable<IEventRecord>> GetEventRecordsAsync(string ownerId)
        {
            var records = await Options.GetCloudTable(AzureTableName)
                                        .ExecuteTableQueryAsync(new TableQuery<EventRecordTableEntity>()
                                                                        .Where(TableQuery.GenerateFilterCondition(
                                                                                            "PartitionKey", 
                                                                                            QueryComparisons.Equal, 
                                                                                            ownerId)));
            return records.OrderBy(r => r.Timestamp);   
        }


        private const string AzureTableName = "stupidtodoevents";
        private readonly AzureTableStorageOptions Options;
    }
}
