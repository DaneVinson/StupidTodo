﻿using Newtonsoft.Json;
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
            IAggregate<ICommand, IEvent, ITodoState> aggregate,
            IEventStore eventStore,
            IMessenger<IEvent> eventMessenger)
        {
            Aggregate = aggregate ?? throw new ArgumentNullException();
            EventMessenger = eventMessenger ?? throw new ArgumentNullException();
            EventStore = eventStore ?? throw new ArgumentNullException();
        }

        public async Task<Result> ExecuteAsync(ICommand command)
        {
            ITodoState state = await HydrateAggregate(command.Id);

            var events = Aggregate.HandleCommand(command, state).ToList();

            // Stop on failed events.
            var failed = events.FirstOrDefault(e => e.EventType == typeof(Failed));
            if (failed != null) { return await EventMessenger.SendMessageAsync(failed); }

            // Save new events.
            events = events.Where(e => e.EventType != typeof(NoOp)).ToList();
            if (!(await EventStore.AddEventRecordsAsync(events.Select(e => e.NewEventRecord(command.Id)))))
            {
                return await EventMessenger.SendMessageAsync(new Failed(command.CommandType, command.Id, "Failed to persist event records."));
            }

            // Apply the events to get current state.
            state = events.Aggregate(state, (s, e) => Aggregate.ApplyEvent(e, s));

            // Emit all created or deleted events resulting.
            var forwardEvents = events
                                    .Where(e => e.EventType == typeof(TodoCreated) || e.EventType == typeof(TodoDeleted))
                                    .ToList();

            // Emit a TodoUpdated event if there were any property changed events.
            if (events.Any(e => e.EventType == typeof(TodoDescriptionChanged) || e.EventType == typeof(TodoDoneChanged)))
            {
                forwardEvents.Add(new TodoUpdated(state.Description, state.Done, state.Id));
            }

            // Send events.
            var results = await Task.WhenAll(forwardEvents.Select(e => EventMessenger.SendMessageAsync(e)));
            if (results.All(r => !r.Success)) { return results.First(); }
            else { return new Result(true); }
        }

        private async Task<ITodoState> HydrateAggregate(Guid id)
        {
            var eventRecords = await EventStore.GetEventRecordsAsync(id);
            var events = eventRecords.Select(e => JsonConvert.DeserializeObject(e.EventData, Type.GetType(e.EventType)) as IEvent);

            ITodoState state = null;
            state = events.Aggregate(state, (s, e) => Aggregate.ApplyEvent(e, s));
            return state;
        }

        private readonly IAggregate<ICommand, IEvent, ITodoState> Aggregate;
        private readonly IMessenger<IEvent> EventMessenger;
        private readonly IEventStore EventStore;
    }
}