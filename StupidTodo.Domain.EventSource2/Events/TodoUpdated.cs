using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct TodoUpdated : IEvent
    {
        public TodoUpdated(string description, bool done, Guid id)
        {
            Description = description;
            Done = done;
            Id = id;
        }

        public string Description;
        public bool Done;
        public Type EventType => GetType();
        public Guid Id;
    }
}
