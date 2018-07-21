using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public interface IEventRecord
    {
        Guid EntityId { get; set; }
        string EventData { get; set; }
        string EventType { get; set; }
        Guid Id { get; set; }
        string Version { get; set; }
    }
}
