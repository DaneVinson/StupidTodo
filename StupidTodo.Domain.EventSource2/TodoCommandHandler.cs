using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public class TodoCommandHandler : ICommandHandler
    {
        public TodoCommandHandler(
            IAggregate aggregate,
            IEventStore eventStore)
        {
            Aggregate = aggregate ?? throw new ArgumentNullException();
            EventStore = eventStore ?? throw new ArgumentNullException();
            Handlers = new Dictionary<Type, Func<ICommand, Task>>
            {
                { typeof(CreateTodoCommand), c => ExecuteAsync((CreateTodoCommand)c) },
                { typeof(DeleteTodoCommand), c => ExecuteAsync((DeleteTodoCommand)c) },
                { typeof(UpdateTodoCommand), c => ExecuteAsync((UpdateTodoCommand)c) }
            };
        }


        public async Task<Result> ExecuteAsync(ICommand command)
        {
            if (Handlers.TryGetValue(command.GetType(), out Func<ICommand, Task> handler))
            {
                await handler(command);
                return new Result() { Success = true };
            }
            else { return new Result() { Message = $"No handler registered for command type {command.GetType()}." }; }
        }


        private async Task<Result> ApplyAndStoreEvents(Guid id, IEnumerable<IEvent> events)
        {
            // Hydrate the aggregate.
            if ((await HydrateAggregate(id))) { return new Result($"Couldn't hydrate {id}"); }

            // Apply the events.
            var results = events.Select(e => Aggregate.ApplyEvent(e));
            if (!results.Any(r => r.Success)) { return new Result("All events failed to apply."); }

            // Attempt to store successfully applied events and emmit further events as needed.
            var records = events.Select(e => e.NewEventRecord(id));
            int i = 0;
            foreach (var result in results)
            {
                var record = records.Skip(i++).First();
                if (await EventStore.AddEventRecordAsync(record) == Guid.Empty)
                {
                    result.Message = $"Add of {record.EventType} with event store {EventStore.GetType()} failed.";
                    result.Success = false;
                }

                // Emmit any requried success or failure events.
                if (result.Success)
                { }
                else
                { }
            }

            if (results.Any(r => r.Success)) { return new Result(true); }
            else { return new Result($"All {events.Count()} failed."); }
        }

        private async Task ExecuteAsync(CreateTodoCommand command)
        {
            var result = await ApplyAndStoreEvents(command.Id, new IEvent[] { new TodoCreated(command.Id, command.Description) });
        }

        private async Task ExecuteAsync(DeleteTodoCommand command)
        {
            var result = await ApplyAndStoreEvents(command.Id, new IEvent[] { new TodoDeleted(command.Id) });
        }

        private async Task ExecuteAsync(UpdateTodoCommand command)
        {
            var events = new List<IEvent>();
            if (command.Description != null) { events.Add(new TodoDescriptionChanged(command.Description)); }
            if (command.Done.HasValue) { events.Add(new TodoDoneChanged(command.Done.Value)); }
            var result = await ApplyAndStoreEvents(command.Id, events);

        }

        private async Task<bool> HydrateAggregate(Guid id)
        {
            var eventRecords = await EventStore.GetEventRecordsAsync(id);
            var events = eventRecords.Select(e => JsonConvert.DeserializeObject(e.EventData, e.EventType) as IEvent);
            var result = Aggregate.Hydrate(events);
            return result.Success;
        }


        private readonly IAggregate Aggregate;
        private readonly IEventStore EventStore;
        private readonly Dictionary<Type, Func<ICommand, Task>> Handlers;
    }
}
