using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public interface IAggregate<TCommand, TEvent, TState>
    {
        TState ApplyEvent(TEvent @event, TState state);
        IEnumerable<TEvent> HandleCommand(TCommand command, TState state);
        TState State { get; set; }
    }
}
