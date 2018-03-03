using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource
{
    public class CreateEventSchema : EventSchema<Created>
    {
        public string Description { get; set; }
        public string Id { get; set; }
    }

    public class UpdateDescriptionEventSchema : EventSchema<DescriptionUpdated>
    {
        public string Description { get; set; }
    }

    public class UpdateDoneEventSchema : EventSchema<DoneUpdated>
    {
        public bool Done { get; set; }
    }
}
