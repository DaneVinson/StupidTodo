using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct DeleteTodoCommand : ICommand
    {
        public DeleteTodoCommand(Guid id)
        {
            Id = id;
        }

        public Type CommandType => GetType();
        public Guid Id { get; set; }
    }
}
