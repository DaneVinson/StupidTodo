using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface ITodoRepository
    {
        Task<Todo> AddTodoAsync(Todo todo);

        Task<bool> DeleteTodoAsync(string userId, string id);

        Task<IEnumerable<Todo>> GetTodosAsync(string userId, bool done = false);

        Task<Todo> UpdateTodoAsync(Todo todo);
    }
}
