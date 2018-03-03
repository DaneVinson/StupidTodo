using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IEventStore
    {
        Task<IEnumerable<string>> GetEventsAsync(string id);
    }
}
