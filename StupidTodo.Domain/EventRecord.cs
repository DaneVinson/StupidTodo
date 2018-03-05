using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class EventRecord : IEventRecord
    {
        public string EventData { get; set; }
        public string EventType { get; set; }
        public string Id { get; set; }
        public string OwnerId { get; set; }
    }
}
