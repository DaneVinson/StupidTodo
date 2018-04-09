using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource
{
    public static class Extensions
    {
        public static EventRecordTableEntity GetEventRecordTableEntity(this IEventRecord eventRecord)
        {
            return new EventRecordTableEntity()
            {
                EventData = eventRecord.EventData,
                HandlerType = eventRecord.HandlerType,
                Id = eventRecord.Id,
                OwnerId = eventRecord.OwnerId,
                PartitionKey = eventRecord.OwnerId,
                RowKey = eventRecord.Id,
            };
        }
    }
}
