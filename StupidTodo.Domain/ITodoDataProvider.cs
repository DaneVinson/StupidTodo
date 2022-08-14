using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface ITodoDataProvider
    {
        Task<bool> Delete(string id);
        Task<IEnumerable<Todo>> Get(bool done = false);
        Task<Todo> Upsert(Todo todo);
    }
}
