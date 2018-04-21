using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public class TodoEntity : IAggregate<ICommand, IEvent, ITodoState>
    {
        public TodoEntity()
        {
            CommandHandlers = new Dictionary<Type, Func<ICommand, IEnumerable<IEvent>>>()
            {
                { typeof(CreateTodoCommand), c => HandleCreate((CreateTodoCommand)c) },
                { typeof(DeleteTodoCommand), c => HandleDelete((DeleteTodoCommand)c) },
                { typeof(UpdateTodoCommand), c => HandleUpdate((UpdateTodoCommand)c) }
            };
            EventHandlers = new Dictionary<Type, Func<IEvent, ITodoState>>()
            {
                { typeof(TodoCreated), e => ApplyCreated((TodoCreated)e) },
                { typeof(TodoDeleted), e => ApplyDeleted((TodoDeleted)e) },
                { typeof(TodoDescriptionChanged), e => ApplyDescriptionChanged((TodoDescriptionChanged)e) },
                { typeof(TodoDoneChanged), e => ApplyDoneChanged((TodoDoneChanged)e) }
            };
        }

        #region IAggregate

        public ITodoState ApplyEvent(IEvent @event, ITodoState state)
        {
            Func<IEvent, ITodoState> handler;
            if (!EventHandlers.TryGetValue(@event.EventType, out handler)) { throw new UnknownHandlerException(); }
            return handler(@event);
        }

        public IEnumerable<IEvent> HandleCommand(ICommand command, ITodoState state)
        {
            if (command == null || command.Id == Guid.Empty) { throw new ArgumentException(); }
            Func<ICommand, IEnumerable<IEvent>> handler;
            if (!CommandHandlers.TryGetValue(command.CommandType, out handler)) { throw new UnknownHandlerException(); }
            return handler(command);
        }

        #endregion

        #region Event Handlers

        private ITodoState ApplyCreated(TodoCreated @event)
        {
            State = new TodoState()
            {
                Description = @event.Description,
                Id = @event.Id
            };
            return State;
        }

        private ITodoState ApplyDeleted(TodoDeleted @event)
        {
            State.Deleted = true;
            return State;
        }

        private ITodoState ApplyDescriptionChanged(TodoDescriptionChanged @event)
        {
            State.Description = @event.Description;
            return State;
        }

        private ITodoState ApplyDoneChanged(TodoDoneChanged @event)
        {
            State.Done = @event.Done;
            return State;
        }

        #endregion

        #region Command Handlers

        private IEnumerable<IEvent> HandleCreate(CreateTodoCommand command)
        {
            // Todo cannot have been created and Description must have a value.
            IEvent @event = null;
            if (State != null) { @event = new Failed(command, "An entity cannot be created twice."); }
            else if (String.IsNullOrWhiteSpace(command.Description)) { @event = new Failed(command, "Description is required."); }
            if (@event != null) { return new IEvent[] { @event }; }

            // Apply the created event and return it.
            var created = new TodoCreated(command.Id, command.Description);
            State = ApplyEvent(created, State);
            return new IEvent[] { created };
        }

        private IEnumerable<IEvent> HandleDelete(DeleteTodoCommand command)
        {
            // Todo must exist and cannot be deleted.
            IEvent @event = null;
            if (State == null) { @event = new Failed(command, "Cannot delete a non-existent entity."); }
            else if (State.Deleted) { @event = new Failed(command, "Entity was previously deleted."); }
            if (@event != null) { return new IEvent[] { @event }; }

            // Apply the deleted event and return it.
            var deleted = new TodoDeleted(command.Id);
            State = ApplyEvent(deleted, State);
            return new IEvent[] { deleted };
        }

        private IEnumerable<IEvent> HandleUpdate(UpdateTodoCommand command)
        {
            var events = new List<IEvent>();

            // Todo must be created and not deleted.
            if (State == null) { events.Add(new Failed(command, "Cannot update a non-existent entity.")); }
            else if (State.Deleted) { events.Add(new Failed(command, "Cannot update a deleted entity.")); }
            if (events.Any()) { return events; }

            // Add changed events as necessary.
            if (!String.IsNullOrWhiteSpace(command.Description) && !command.Description.Equals(State.Description))
            {
                events.Add(new TodoDescriptionChanged(command.Description));
            }
            if (command.Done.HasValue && command.Done.Value != State.Done)
            {
                events.Add(new TodoDoneChanged(command.Done.Value));
            }

            // Apply events.
            State = events.Aggregate(State, (s, e) => ApplyEvent(e, s));

            return events;
        }

        #endregion

        public ITodoState State { get; set; }


        private readonly Dictionary<Type, Func<ICommand, IEnumerable<IEvent>>> CommandHandlers;
        private readonly Dictionary<Type, Func<IEvent, ITodoState>> EventHandlers;
    }
}
