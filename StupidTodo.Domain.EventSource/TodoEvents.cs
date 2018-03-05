using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IEvent
    {
        Todo Execute(string jsonSchema, Todo todo);
    }

    public class Created : IEvent
    {
        public Todo Execute(string jsonSchema, Todo ignored = null)
        {
            var schema = JsonConvert.DeserializeObject<CreateEventSchema>(jsonSchema);
            if (schema == null) { return null; }

            return new Todo()
            {
                Description = schema.Description,
                Done = false,
                Id = schema.Id
            };
        }
    }

    public class Deleted : IEvent
    {
        public Todo Execute(string jsonSchema, Todo todo)
        {
            if (todo == null) { return null; }
            var schema = JsonConvert.DeserializeObject<DeleteEventSchema>(jsonSchema);
            return todo;
        }
    }

    public class DescriptionUpdated : IEvent
    {
        public Todo Execute(string jsonSchema, Todo todo)
        {
            if (todo == null) { return null; }
            var schema = JsonConvert.DeserializeObject<UpdateDescriptionEventSchema>(jsonSchema);
            if (schema == null) { return todo; }

            todo.Description = schema.Description;
            return todo;
        }
    }

    public class DoneUpdated : IEvent
    {
        public Todo Execute(string jsonSchema, Todo todo)
        {
            if (todo == null) { return null; }
            var schema = JsonConvert.DeserializeObject<UpdateDoneEventSchema>(jsonSchema);
            if (schema == null) { return todo; }

            todo.Done = schema.Done;
            return todo;
        }
    }
}
