using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public class TodoEntity : IAggregate
    {
        public TodoEntity()
        {
            EventHandlers = new Dictionary<Type, Func<IEvent, Result>>()
            {
                { typeof(TodoCreated), e => Apply((TodoCreated)e) },
                { typeof(TodoDeleted), e => Apply((TodoDeleted)e) },
                { typeof(TodoDescriptionChanged), e => Apply((TodoDescriptionChanged)e) },
                { typeof(TodoDoneChanged), e => Apply((TodoDoneChanged)e) }
            };
        }

        #region IAggregate

        public Result ApplyEvent(IEvent e)
        {
            var result = EventHandlers[e.EventType](e);
            if (result.Success) { UncommittedEvents.Add(e); }
            return result;
        }

        public IEnumerable<IEvent> GetUncommittedEvents() => UncommittedEvents;

        public Result Hydrate(IEnumerable<IEvent> events)
        {
            ResetEntity();

            // Pre-validate the events before accepting.
            if (events.Count() > 0 && events.First().EventType != typeof(TodoCreated)) { return new Result() { Message = "The first event must be created." }; }
            if (events.Where(e => e.EventType == typeof(TodoCreated)).Count() > 1) { return new Result() { Message = "There can be only 1 created event." }; }
            var deletedEvents = events.Where(e => e.EventType == typeof(TodoDeleted)).ToArray();
            if (deletedEvents.Length > 0 && events.Last().EventType != typeof(TodoDeleted)) { return new Result() { Message = "Deleted must be the last event." }; }
            else if (deletedEvents.Length > 1) { return new Result() { Message = "There can be only 1 deleted event." }; }

            foreach (IEvent e in events)
            {
                if (EventHandlers[e.EventType](e).Success) { CommittedEvents.Add(e); }
            }

            return new Result() { Success = true };
        }

        #endregion

        private Result Apply(TodoCreated e)
        {
            // Validate event: created event should be first
            if (CommittedEvents.Any() || UncommittedEvents.Any()) { return new Result($"{nameof(e.EventType)} must be the first event."); }

            // Apply event
            Description = e.Description;
            Id = e.Id;
            return new Result(true);
        }

        private Result Apply(TodoDeleted e)
        {
            // Validate event: Id requried, not Deleted
            if (Id == Guid.Empty) { return new Result("Entity has not been created."); }
            if (Deleted) { return new Result("Entity was previously deleted."); }

            // Apply event
            Deleted = true;
            return new Result(true);
        }

        private Result Apply(TodoDescriptionChanged e)
        {
            // Validate event: Id requried, not Deleted, Description required, Description must have changed
            if (Id == Guid.Empty) { return new Result("Entity has not been created."); }
            if (Deleted) { return new Result("Entity was previously deleted."); }
            if (String.IsNullOrWhiteSpace(e.Description)) { return new Result("Description must have a value."); }
            if (e.Description.Equals(Description)) { return new Result($"Description is already {Description}."); }

            // Apply event
            Description = e.Description;
            return new Result(true);
        }

        private Result Apply(TodoDoneChanged e)
        {
            // Validate event: Id requried, not Deleted, Done must have chaged
            if (Id == Guid.Empty) { return new Result("Entity has not been created."); }
            if (Deleted) { return new Result("Entity has been deleted."); }
            if (Done = e.Done) { return new Result($"Done is already {Done}"); }

            // Apply event
            Done = e.Done;
            return new Result(true);
        }

        private void ResetEntity()
        {
            CommittedEvents.Clear();
            UncommittedEvents.Clear();
            Deleted = false;
            Description = null;
            Done = false;
            Id = Guid.Empty;
        }


        public bool Deleted { get; private set; }
        public string Description { get; private set; }
        public bool Done { get; private set; }
        public Guid Id { get; private set; }


        private readonly List<IEvent> CommittedEvents = new List<IEvent>();
        private readonly Dictionary<Type, Func<IEvent, Result>> EventHandlers;
        private readonly List<IEvent> UncommittedEvents = new List<IEvent>();
    }
}
