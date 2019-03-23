using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public abstract class Command : ICommand
    {
        public Type CommandType => GetType();
    }
}
