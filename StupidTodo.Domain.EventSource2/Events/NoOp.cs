using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct NoOp : IEvent
    {
        public Type EventType => GetType();
    }
}
