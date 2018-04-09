using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public interface ICommand
    {
        Guid Id { get; }
        Type CommandType { get; }
    }
}
