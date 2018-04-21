using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class TodoState : ITodoState
    {
        public bool Deleted { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public Guid Id { get; set; }
    }
}
