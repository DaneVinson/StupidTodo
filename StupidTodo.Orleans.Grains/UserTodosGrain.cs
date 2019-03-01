using Orleans;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Orleans.Grains
{
    public class UserTodosGrain : Grain, IUserTodosGrain
    {
        public UserTodosGrain(ITodoRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException();
            Todos = new List<Todo>();
        }


        public override async Task OnActivateAsync()
        {
            Todos.AddRange(await Repository.GetTodosAsync(this.GetPrimaryKeyString()));
            await base.OnActivateAsync();
        }

        #region IUserTodosGrain

        public async Task<Todo> AddTodoAsync(Todo todo)
        {
            var todos = await Repository.AddTodosAsync(new Todo[] { todo });
            todo = todos?.FirstOrDefault();
            if (todo != null) { Todos.Add(todo); }
            return todo;
        }

        public async Task<bool> DeleteTodoAsync(string id)
        {
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null) { return false; }
            var success = await Repository.DeleteTodoAsync(todo.UserId, id);
            if (success) { Todos.Remove(todo); }
            return success;
        }

        public Task<IEnumerable<Todo>> GetTodosAsync(bool done = false)
        {
            return Task.FromResult(Todos.Where(t => t.Done == done));
        }

        public async Task<Todo> UpdateTodoAsync(Todo todo)
        {
            var todos = await Repository.UpdateTodosAsync(new Todo[] { todo });
            todo = todos?.FirstOrDefault();
            if (todo != null) { Todos.Add(todo); }
            return todo;
        }

        #endregion

        private readonly List<Todo> Todos;
        private readonly ITodoRepository Repository;
    }
}
