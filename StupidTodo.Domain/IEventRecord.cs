using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public interface IEventRecord
    {
        string EventData { get; set; }
        string EventType { get; set; }
        string Id { get; set; }
        string OwnerId { get; set; }
    }
}
