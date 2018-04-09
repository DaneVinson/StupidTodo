using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct TodoCreated : IEvent
    {
        public TodoCreated(Guid id, string description)
        {
            Description = description;
            Id = id;
        }

        public string Description;
        public Type EventType => GetType();
        public Guid Id;
    }
}
