using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public interface IEventStore
    {
        Task<Guid> AddEventRecordAsync(IEventRecord eventRecord);
        Task<IEnumerable<IEventRecord>> GetEventRecordsAsync(Guid entityId);
    }
}
