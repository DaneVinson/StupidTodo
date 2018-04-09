using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.EventSource2
{
    public interface IAggregate
    {
        Result ApplyEvent(IEvent e);
        IEnumerable<IEvent> GetUncommittedEvents();
        Result Hydrate(IEnumerable<IEvent> events);
    }
}
