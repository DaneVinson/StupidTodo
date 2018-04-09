using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public static class Extensions
    {
        public static EventRecordTableEntity NewEventRecordTableEntity(this TodoCreated todoCreated, Version version = null)
        {
            var recordId = Guid.NewGuid();
            return new EventRecordTableEntity()
            {
                EntityId = todoCreated.Id,
                EventData = JsonConvert.SerializeObject(todoCreated),
                EventType = todoCreated.GetType(),
                Id = recordId,
                PartitionKey = todoCreated.Id.ToString(),
                RowKey = recordId.ToString(),
                Version = version ?? new Version(1, 0, 0, 0)
            };
        }

        public static EventRecordTableEntity NewEventRecordTableEntity(this TodoDeleted todoDeleted, Version version = null)
        {
            var recordId = Guid.NewGuid();
            return new EventRecordTableEntity()
            {
                EntityId = todoDeleted.Id,
                EventData = JsonConvert.SerializeObject(todoDeleted),
                EventType = todoDeleted.GetType(),
                Id = recordId,
                PartitionKey = todoDeleted.Id.ToString(),
                RowKey = recordId.ToString(),
                Version = version ?? new Version(1, 0, 0, 0)
            };
        }

        public static EventRecordTableEntity NewEventRecordTableEntity(this TodoDescriptionChanged todoDescriptionChanged, Guid entityId, Version version = null)
        {
            var recordId = Guid.NewGuid();
            return new EventRecordTableEntity()
            {
                EntityId = entityId,
                EventData = JsonConvert.SerializeObject(todoDescriptionChanged),
                EventType = todoDescriptionChanged.GetType(),
                Id = recordId,
                PartitionKey = entityId.ToString(),
                RowKey = recordId.ToString(),
                Version = version ?? new Version(1, 0, 0, 0)
            };
        }

        public static EventRecordTableEntity NewEventRecordTableEntity(this TodoDoneChanged todoDoneChanged, Guid entityId, Version version = null)
        {
            var recordId = Guid.NewGuid();
            return new EventRecordTableEntity()
            {
                EntityId = entityId,
                EventData = JsonConvert.SerializeObject(todoDoneChanged),
                EventType = todoDoneChanged.GetType(),
                Id = recordId,
                PartitionKey = entityId.ToString(),
                RowKey = recordId.ToString(),
                Version = version ?? new Version(1, 0, 0, 0)
            };
        }

        public static EventRecordTableEntity NewEventRecord(this IEvent evnt, Guid entityId, Version version = null)
        {
            var recordId = Guid.NewGuid();
            return new EventRecordTableEntity()
            {
                EntityId = entityId,
                EventData = JsonConvert.SerializeObject(evnt),
                EventType = evnt.EventType,
                Id = recordId,
                PartitionKey = entityId.ToString(),
                RowKey = recordId.ToString(),
                Version = version ?? new Version(1, 0, 0, 0)
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
                Version = version ?? new Version(1, 0, 0, 0)
            };
        }

    }
}
