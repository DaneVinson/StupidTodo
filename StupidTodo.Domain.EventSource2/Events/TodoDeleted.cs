using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct TodoDeleted : IEvent
    {
        public TodoDeleted(Guid id)
        {
            Id = id;
        }

        public Type EventType => GetType();
        public Guid Id;
    }
}
