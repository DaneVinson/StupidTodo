using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface ITodoApi
    {
        Task<Todo?> AddTodoAsync(Todo todo);
        Task<bool> DeleteTodoAsync(string id);
        Task<Todo[]?> GetTodosAsync(bool done = false);
        Task<Todo?> UpdateTodoAsync(Todo todo);
    }
}
