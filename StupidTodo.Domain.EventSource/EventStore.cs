using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
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
        public AzureTableStorageEventStore(AzureTableStorageEventStoreOptions options)
        {
            Options = options;
        }


        public async Task<string> AddEventRecordAsync(IEventRecord eventRecord)
        {
            var tableResult = await NewCloudTable().ExecuteAsync(TableOperation.Insert(eventRecord.GetEventTableRecord()));
            if (tableResult.HttpStatusCode.IsSuccessCode()) { return eventRecord.Id; }
            else { return null; }
        }

        public async Task<IEnumerable<IEventRecord>> GetEventRecordsAsync(string ownerId)
        {
            var records = await NewCloudTable()
                                    .ExecuteTableQueryAsync(new TableQuery<EventTableRecord>()
                                                                    .Where(TableQuery.GenerateFilterCondition(
                                                                                        "PartitionKey", 
                                                                                        QueryComparisons.Equal, 
                                                                                        ownerId)));
            return records.OrderBy(r => r.Timestamp);   
        }


        private CloudTable NewCloudTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={Options.AccountName};AccountKey={Options.Key}");
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("stupidtodo");
        }

        private readonly AzureTableStorageEventStoreOptions Options;
    }

    public class AzureTableStorageEventStoreOptions
    {
        public string AccountName { get; set; }
        public string Key { get; set; }
    }
}
