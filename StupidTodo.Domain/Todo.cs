using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class Todo
    {
        public string Description { get; set; }
        public bool Done { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }

        public override string ToString()
        {
            return $"{Description} (Id: {Id}, Done: {Done})";
        }
    }
}
