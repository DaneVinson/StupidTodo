using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetTodosAsync(string userId);
        Task<bool> PersistTodosAsync(string userId, IEnumerable<Todo> todos);
    }
}
