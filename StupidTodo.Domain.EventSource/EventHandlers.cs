using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IEventHandler<TEntity> where TEntity : new()
    {
        TEntity ApplyEvent(TEntity todo, string eventJson);
    }


    public class CreatedEventHandler: IEventHandler<Todo>
    {
        public Todo ApplyEvent(Todo todo, string eventJson)
        {
            var createdEvent = JsonConvert.DeserializeObject<CreatedEvent>(eventJson);
            if (createdEvent == null) { return null; }

            return new Todo()
            {
                Description = createdEvent.Description,
                Done = false,
                Id = createdEvent.Id
            };
        }
    }

    public class DeletedEventHandler : IEventHandler<Todo>
    {
        public Todo ApplyEvent(Todo todo, string eventJson)
        {
            return todo;
        }
    }

    public class DescriptionUpdatedEventHandler : IEventHandler<Todo>
    {
        public Todo ApplyEvent(Todo todo, string eventJson)
        {
            if (todo == null) { return null; }
            var updatedEvent = JsonConvert.DeserializeObject<DescriptionUpdatedEvent>(eventJson);
            if (updatedEvent == null) { return null; }
            todo.Description = updatedEvent.Description;
            return todo;
        }
    }

    public class DoneUpdatedEventHandler : IEventHandler<Todo>
    {
        public Todo ApplyEvent(Todo todo, string eventJson)
        {
            if (todo == null) { return null; }
            var updatedEvent = JsonConvert.DeserializeObject<DoneUpdatedEvent>(eventJson);
            if (updatedEvent == null) { return null; }
            todo.Done = updatedEvent.Done;
            return todo;
        }
    }
}
