using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource
{
    public class CreatedEvent
    {
        public string Description { get; set; }
        public string Id { get; set; }
    }

    public class DeletedEvent
    {
        public string Id { get; set; }
    }

    public class DescriptionUpdatedEvent
    {
        public string Description { get; set; }
    }

    public class DoneUpdatedEvent
    {
        public bool Done { get; set; }
    }
}
