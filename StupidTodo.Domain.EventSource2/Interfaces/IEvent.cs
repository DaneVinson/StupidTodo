using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public interface IEvent
    {
        Type EventType { get; }
    }
}
