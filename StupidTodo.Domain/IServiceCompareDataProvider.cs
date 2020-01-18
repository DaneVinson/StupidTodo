using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface IServiceCompareDataProvider : ITodoDataProvider
    {
        Task<IEnumerable<Todo>> Get();
        Task<Todo> GetFirst();
        Task<bool> Send(IEnumerable<Todo> todos);
        Task<bool> Send(Todo todo);
    }
}
