using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public class Created : Event<Todo>
    {
        public override Todo Execute(string jsonSchema, Todo ignoredTodo = null)
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

    public class DescriptionUpdated : Event<Todo>
    {
        public override Todo Execute(string jsonSchema, Todo todo)
        {
            if (todo == null) { return null; }
            var schema = JsonConvert.DeserializeObject<UpdateDescriptionEventSchema>(jsonSchema);
            if (schema == null) { return todo; }

            todo.Description = schema.Description;
            return todo;
        }
    }

    public class DoneUpdated : Event<Todo>
    {
        public override Todo Execute(string jsonSchema, Todo todo)
        {
            if (todo == null) { return null; }
            var schema = JsonConvert.DeserializeObject<UpdateDoneEventSchema>(jsonSchema);
            if (schema == null) { return todo; }

            todo.Done = schema.Done;
            return todo;
        }
    }
}
