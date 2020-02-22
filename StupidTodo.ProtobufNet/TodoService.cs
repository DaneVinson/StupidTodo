using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.ProtobufNet
{
    public class TodoService : ITodoService
    {
        public Task<bool> DeleteTodo(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Todo>> GetDoneTodos()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Todo>> GetTodos()
        {
            throw new NotImplementedException();
        }

        public Task<Todo> UpsertTodo(Todo todo)
        {
            throw new NotImplementedException();
        }
    }
}
