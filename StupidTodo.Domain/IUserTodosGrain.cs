using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface IUserTodosGrain : IGrainWithStringKey
    {
        Task<Todo> AddTodoAsync(Todo todo);

        Task<bool> DeleteTodoAsync(string id);

        Task<IEnumerable<Todo>> GetTodosAsync(bool done = false);

        Task<Todo> UpdateTodoAsync(Todo todo);
    }
}
