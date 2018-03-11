using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface ICommandHandler
    {
        Task ExecuteCommandAsync(string commandJson);
    }

    public abstract class BaseCommandHandler<TEntity> where TEntity : new()
    {
        protected BaseCommandHandler(
            IEventStore eventStore,
            IProjector<TEntity> projector)
        {
            EventStore = eventStore ?? throw new ArgumentNullException();
            Projector = projector ?? throw new ArgumentNullException();
        }

        protected async Task<TEntity> GetEntityAsync(string id)
        {
            // Get the events records from storage
            var eventRecords = await EventStore.GetEventRecordsAsync(id);
            if (eventRecords == null || eventRecords.Count() == 0) { return default(TEntity); }

            // Hydrate entity using the events
            var entity = new TEntity();
            foreach (var eventRecord in eventRecords)
            {
                entity = (Activator.CreateInstance(Type.GetType(eventRecord.HandlerType)) as IEventHandler<TEntity>)
                                    .ApplyEvent(entity, eventRecord.EventData);
            }

            return entity;
        }

        protected async Task<Result<string>> StoreEventAsync<THandler>(string ownerId, object eventObject)
        {
            var eventRecord = new EventRecord()
            {
                EventData = JsonConvert.SerializeObject(eventObject),
                HandlerType = typeof(THandler).ToString(),
                Id = Guid.NewGuid().ToString(),
                OwnerId = ownerId
            };

            var result = new Result<string>();
            var id = await EventStore.AddEventRecordAsync(eventRecord);
            if (String.IsNullOrWhiteSpace(id)) { result.Message = "Add to event store failed."; }
            else
            {
                result.Success = true;
                result.Value = id;
            }

            return result;
        }

        protected readonly IEventStore EventStore;
        protected readonly IProjector<TEntity> Projector;
    }

    public class CreateCommandHandler : BaseCommandHandler<Todo>, ICommandHandler
    {
        public CreateCommandHandler(
            IEventStore eventStore, 
            IProjector<Todo> projector) : base(eventStore, projector)
        { }

        public async Task ExecuteCommandAsync(string commandJson)
        {
            var command = JsonConvert.DeserializeObject<CreateCommand>(commandJson);
            if (command == null) { throw new ArgumentException(); }

            var createdEvent = new CreatedEvent()
            {
                Description = command.Description,
                Id = Guid.NewGuid().ToString()
            };

            var result = await StoreEventAsync<CreatedEventHandler>(createdEvent.Id, createdEvent);
            if (result.Success)
            {
                var todo = await GetEntityAsync(createdEvent.Id);
                await Projector.PersistProjectionAsync(todo);
            }
        }
    }

    public class DeleteCommandHandler : BaseCommandHandler<Todo>, ICommandHandler
    {
        public DeleteCommandHandler(
            IEventStore eventStore,
            IProjector<Todo> projector) : base(eventStore, projector)
        { }

        public async Task ExecuteCommandAsync(string commandJson)
        {
            var command = JsonConvert.DeserializeObject<DeleteCommand>(commandJson);
            if (command == null || command.TargetId == null) { throw new ArgumentException(); }

            var deletedEvent = new DeletedEvent() { Id = command.TargetId };

            var result = await StoreEventAsync<DeletedEventHandler>(deletedEvent.Id, deletedEvent);
            if (result.Success)
            {
                await Projector.RemoveProjectionAsync(new Todo() { Id = command.TargetId });
            }
        }
    }

    public class UpdateCommandHandler : BaseCommandHandler<Todo>, ICommandHandler
    {
        public UpdateCommandHandler(
            IEventStore eventStore,
            IProjector<Todo> projector) : base(eventStore, projector)
        { }

        public async Task ExecuteCommandAsync(string commandJson)
        {
            var command = JsonConvert.DeserializeObject<UpdateCommand>(commandJson);
            if (command == null || command.TargetId == null) { throw new ArgumentException(); }

            var tasks = new List<Task<Result<string>>>();
            if (command.Description != null)
            {
                var descriptionUpdatedEvent = new DescriptionUpdatedEvent() { Description = command.Description };
                tasks.Add(StoreEventAsync<DescriptionUpdatedEventHandler>(command.TargetId, descriptionUpdatedEvent));
            }
            if (command.Done != null)
            {
                var doneUpdatedEvent = new DoneUpdatedEvent() { Done = command.Done.Value };
                tasks.Add(StoreEventAsync<DoneUpdatedEventHandler>(command.TargetId, doneUpdatedEvent));
            }
            var results = (await Task.WhenAll(tasks))?
                                        .Where(r => r != null)
                                        .ToArray();
            if (results.Any(r => r.Success))
            {
                var todo = await GetEntityAsync(command.TargetId);
                await Projector.PersistProjectionAsync(todo);
            }

        }
    }
}
