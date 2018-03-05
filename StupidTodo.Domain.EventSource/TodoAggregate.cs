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
        // TODO: Push IProjector dependency to queue abstraction.
        public TodoAggregate(IEventStore eventStore, IProjector<Todo> projector)
        {
            EventStore = eventStore ?? throw new ArgumentNullException();
            TodoProjector = projector ?? throw new ArgumentNullException();
        }


        public async Task<Result<string>> AddEvent(string todoId, Type eventType, string jsonSchema)
        {
            var eventRecord = new EventRecord()
            {
                EventData = jsonSchema,
                EventType = eventType.ToString(),
                Id = Guid.NewGuid().ToString(),
                OwnerId = todoId
            };

            var result = new Result<string>();
            var id = await EventStore.AddEventRecordAsync(eventRecord);
            if (String.IsNullOrWhiteSpace(id)) { result.Message = "Add to event store failed."; }
            else
            {
                result.Success = true;
                result.Value = id;
                await QueueProjectionAsync(eventRecord.OwnerId);
            }

            return result;
        }

        public async Task<Todo> GetTodoAsyc(string todoId)
        {
            // Get the events records from storage
            var eventRecords = await EventStore.GetEventRecordsAsync(todoId);
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


        private async Task QueueProjectionAsync(string id)
        {
            // TODO: Queue this task using queue abstraction
            var result = await TodoProjector.ProjectAsync(await GetTodoAsyc(id));
        }

        private readonly IEventStore EventStore;
        private readonly IProjector<Todo> TodoProjector;
    }
}
