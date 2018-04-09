using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct TodoDescriptionChanged : IEvent
    {
        public TodoDescriptionChanged(string description)
        {
            Description = description;
        }

        public string Description;
        public Type EventType => GetType();
    }
}
