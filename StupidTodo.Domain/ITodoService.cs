using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    [ProtoContract]
    public interface ITodoService
    {
        Task<bool> DeleteTodo(string id);

        Task<IEnumerable<Todo>> GetDoneTodos();

        Task<IEnumerable<Todo>> GetTodos();

        Task<Todo> UpsertTodo(Todo todo);
    }
}
