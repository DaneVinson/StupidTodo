using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public class TodoAggregate
    {
        public TodoAggregate(IEventStore eventStore)
        {
            EventStore = eventStore;
        }


        public async Task<string> AddEvent(string todoId, Type eventType, string jsonSchema)
        {
            var eventRecord = new EventRecord()
            {
                EventData = jsonSchema,
                EventType = eventType.ToString(),
                Id = Guid.NewGuid().ToString(),
                OwnerId = todoId
            };

            return await EventStore.AddEventRecordAsync(eventRecord);
        }

        public async Task<Todo> GetTodoAsyc(string ownerId)
        {
            // Get the events records from storage
            var eventRecords = await EventStore.GetEventRecordsAsync(ownerId);
            if (eventRecords == null || eventRecords.Count() == 0) { return null; }

            // Hydrate a Todo using the events
            var todo = new Todo();
            foreach (var eventRecord in eventRecords)
            {
                // TODO: cache event instances
                todo = (Activator.CreateInstance(Type.GetType(eventRecord.EventType)) as IEvent)
                                    .Execute(eventRecord.EventData, todo);
            }

            return todo;
        }


        private readonly IEventStore EventStore;
    }
}
