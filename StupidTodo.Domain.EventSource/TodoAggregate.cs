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


        public async Task<Todo> GetTodoAsyc(string id)
        {
            // Get the events from storage
            var eventSchemas = await EventStore.GetEventsAsync(id);
            if (eventSchemas == null || eventSchemas.Count() == 0) { return null; }

            // Hydrate a Todo using the events
            var todo = new Todo();
            foreach (var eventSchemaJson in eventSchemas)
            {
                // Get the event type
                var eventSchema = JsonConvert.DeserializeObject<EventSchema<Event<Todo>>>(eventSchemaJson);

                var eventTypeName = eventSchema.EventTypeName;
                if (String.IsNullOrWhiteSpace(eventTypeName)) { continue; }

                // Get the event
                var t = Type.GetType(eventTypeName);
                var todoEvent = (Event<Todo>)Activator.CreateInstance(Type.GetType(eventTypeName));
                if (todoEvent == null) { continue; }

                // Execute
                todo = todoEvent.Execute(eventSchemaJson, todo);
            }

            return todo;
        }


        private readonly IEventStore EventStore;
    }
}
