using Orleans;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Orleans.Grains
{
    public class UsersTodoGrain : Grain, IUsersTodoGrain
    {
        public UsersTodoGrain(ITodoRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException();
            Todos = new List<Todo>();
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
        }


        public Task<Todo> AddTodoAsync(Todo todo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTodoAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Todo>> GetTodosAsync(bool done = false)
        {
            throw new NotImplementedException();
        }

        public Task<Todo> UpdateTodoAsync(Todo todo)
        {
            throw new NotImplementedException();
        }


        private readonly List<Todo> Todos;
        private readonly ITodoRepository Repository;
    }
}
