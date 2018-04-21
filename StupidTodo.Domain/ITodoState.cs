using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public interface ITodoState
    {
        bool Deleted { get; set; }
        string Description { get; set; }
        bool Done { get; set; }
        Guid Id { get; set; }
    }
}
