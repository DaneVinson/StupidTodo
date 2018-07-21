using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public class CommandEventsDispatcher : IDispatcher<IEvent>
    {
        public CommandEventsDispatcher(IProjector<ITodo, bool> todoProjector)
        {
            TodoProjector = todoProjector ?? throw new ArgumentNullException();
            Handlers = new Dictionary<Type, Func<IEvent, Task<Result>>>()
            {
                { typeof(TodoCreated), e => DispatchCreatedEvent((TodoCreated)e) },
                { typeof(TodoDeleted), e => DispatchDeletedEvent((TodoDeleted)e) },
                { typeof(Failed), e => DispatchFailedEvent((Failed)e) },
                { typeof(NoOp), e => DispatchNoOpEvent((NoOp)e) },
                { typeof(TodoUpdated), e => DispatchUpdatedEvent((TodoUpdated)e) }
            };
        }


        public async Task<Result> DispatchAsync(string message)
        {
            // Deserialize the message as an event.
            var eventType = JObject.Parse(message)?["EventType"]?.ToString();
            if (String.IsNullOrWhiteSpace(eventType)) { return new Result("The message was not an event."); }
            var type = Type.GetType(eventType);
            if (type == null) { return new Result("The message was not a known type."); }
            var @event = JsonConvert.DeserializeObject(message, type) as IEvent;

            // Handle the event.
            Func<IEvent, Task<Result>> handler;
            if (!Handlers.TryGetValue(@event.EventType, out handler)) { handler = e => DispatchNoOpEvent((IEvent)e); }
            return await handler(@event);
        }


        private async Task<Result> DispatchCreatedEvent(TodoCreated @event)
        {
            var todo = new Todo()
            {
                Description = @event.Description,
                Id = @event.Id.ToString()
            };
            return await TodoProjector.PersistProjectionAsync(todo);
        }

        private async Task<Result> DispatchDeletedEvent(TodoDeleted @event)
        {
            return await TodoProjector.RemoveProjectionAsync(new Todo() { Id = @event.Id.ToString() });
        }

        private Task<Result> DispatchFailedEvent(Failed @event)
        {
            // TODO: Log Failed events
            return Task.FromResult(new Result(true));
        }

        private Task<Result> DispatchNoOpEvent(IEvent @event)
        {
            return Task.FromResult(new Result(true));
        }

        private async Task<Result> DispatchUpdatedEvent(TodoUpdated @event)
        {
            var todo = new Todo()
            {
                Description = @event.Description,
                Done = @event.Done,
                Id = @event.Id.ToString()
            };
            return await TodoProjector.PersistProjectionAsync(todo);
        }


        private readonly Dictionary<Type, Func<IEvent, Task<Result>>> Handlers;
        private readonly IProjector<ITodo, bool> TodoProjector;
    }
}
