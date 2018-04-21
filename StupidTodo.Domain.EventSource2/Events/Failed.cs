using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public struct Failed : IEvent
    {
        public Failed(Type failedType, Guid id, string message)
        {
            FailedType = failedType;
            Id = id;
            Message = message;
        }

        public Failed(ICommand command, string message)
        {
            FailedType = command.CommandType;
            Id = command.Id;
            Message = message;
        }

        public Type EventType => GetType();
        public Type FailedType;
        public Guid Id;
        public string Message;
    }
}
