using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> AddTodosAsync(IEnumerable<Todo> todo);

        Task<bool> DeleteTodoAsync(string userId, string id);

        Task<IEnumerable<Todo>> GetTodosAsync(string userId);

        Task<IEnumerable<Todo>> UpdateTodosAsync(IEnumerable<Todo> todos);
    }
}
