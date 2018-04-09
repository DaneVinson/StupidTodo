using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct UpdateTodoCommand : ICommand
    {
        public UpdateTodoCommand(Guid id, string description, bool? done)
        {
            Description = description;
            Done = done;
            Id = id;
        }

        public Type CommandType => GetType();
        public string Description;
        public bool? Done;
        public Guid Id { get; set; }
    }
}
