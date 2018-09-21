using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public interface ITodo
    {
        string Description { get; set; }
        bool Done { get; set; }
        string Id { get; set; }
    }
}
