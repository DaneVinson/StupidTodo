using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct CreateTodoCommand : ICommand
    {
        public CreateTodoCommand(string description)
        {
            Description = description;
            Id = Guid.NewGuid();
        }

        public Type CommandType => GetType();
        public string Description;
        public Guid Id { get; set; }
    }
}
