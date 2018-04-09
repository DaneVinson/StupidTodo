using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct TodoDoneChanged : IEvent
    {
        public TodoDoneChanged(bool done)
        {
            Done = done;
        }

        public bool Done;
        public Type EventType => GetType();
    }
}
