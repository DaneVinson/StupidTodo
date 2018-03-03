using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource
{
    public class EventSchema<TEvent>
    {
        public string EventTypeName => GetType().ToString();
    }
}
