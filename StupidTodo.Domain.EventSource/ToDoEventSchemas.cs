using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource
{
    public class CreateEventSchema
    {
        public string Description { get; set; }
        public string Id { get; set; }
    }

    public class UpdateDescriptionEventSchema
    {
        public string Description { get; set; }
    }

    public class UpdateDoneEventSchema
    {
        public bool Done { get; set; }
    }
}
