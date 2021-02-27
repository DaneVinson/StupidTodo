using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public interface ITodo
    {
        string Description { get; }
        bool Done { get; }
        string Id { get; }
        string UserId { get; }
    }
}
