using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public static class Extensions
    {
        public static EventRecordTableEntity NewEventRecord(this IEvent evnt, Guid entityId, Version version = null)
        {
            var recordId = Guid.NewGuid();
            return new EventRecordTableEntity()
            {
                EntityId = entityId,
                EventData = JsonConvert.SerializeObject(evnt),
                EventType = evnt.EventType.ToString(),
                Id = recordId,
                PartitionKey = entityId.ToString(),
                RowKey = recordId.ToString(),
                Version = version?.ToString() ?? new Version(1, 0, 0, 0).ToString()
            };
        }

        public static EventRecordTableEntity GetEventRecordTableEntity(this IEventRecord eventRecord, Version version = null)
        {
            return new EventRecordTableEntity()
            {
                EntityId = eventRecord.EntityId,
                EventData = eventRecord.EventData,
                EventType = eventRecord.EventType,
                Id = eventRecord.Id,
                PartitionKey = eventRecord.EntityId.ToString(),
                RowKey = eventRecord.Id.ToString(),
                Version = version?.ToString() ?? new Version(1, 0, 0, 0).ToString()
            };
        }

    }
}
