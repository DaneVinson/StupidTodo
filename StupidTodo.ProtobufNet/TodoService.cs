using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.ProtobufNet
{
    public class TodoService : ITodoService
    {
        public TodoService(ITodoDataProvider dataProvider)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException();
        }


        public Task<bool> DeleteTodo(string id)
        {
            return DataProvider.Delete(id);
        }

        public Task<IEnumerable<Todo>> GetDoneTodos()
        {
            return DataProvider.Get(true);
        }

        public Task<IEnumerable<Todo>> GetTodos()
        {
            return DataProvider.Get();
        }

        public Task<Todo> UpsertTodo(Todo todo)
        {
            return DataProvider.Upsert(todo);
        }


        private readonly ITodoDataProvider DataProvider;
    }
}
