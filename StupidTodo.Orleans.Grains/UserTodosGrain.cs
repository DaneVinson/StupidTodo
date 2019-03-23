using Orleans;
using Orleans.Streams;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Orleans.Grains
{
    [ImplicitStreamSubscription("StupidTodo")]
    public class UserTodosGrain : Grain, IUserTodosGrain
    {
        public UserTodosGrain(ITodoRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException();
            Todos = new List<Todo>();
        }


        public override async Task OnActivateAsync()
        {
            // We're using a string for the grain's primary key but the stream providers's GetStream method
            // requires a GUID thus we convert primary key from string to GUID
            var stream = GetStreamProvider("StupidTodoStreamProvider")
                            .GetStream<string>(
                                new Guid(MD5.Create().ComputeHash(Encoding.Default.GetBytes(this.GetPrimaryKeyString()))), 
                                "StupidTodo");

            // Get Todos and then listen for new events.
            Todos.AddRange(await Repository.GetTodosAsync(this.GetPrimaryKeyString()));
            await stream.SubscribeAsync(async (data, token) => await HandleEventAsync(data, token));

            await base.OnActivateAsync();
        }

        private Task HandleEventAsync(string data, StreamSequenceToken token)
        {
            // deserilize data then handle the specific type.

            throw new NotImplementedException();
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
            return Task.FromResult(Todos.Where(t => t.Done == done).ToArray().AsEnumerable());
        }

        public async Task<Todo> UpdateTodoAsync(Todo todo)
        {
            var todos = await Repository.UpdateTodosAsync(new Todo[] { todo });
            todo = todos?.FirstOrDefault();
            if (todo != null)
            {
                var currentTodo = Todos.FirstOrDefault(t => t.Id == todo.Id);
                if (currentTodo == null) { Todos.Add(todo); }
                else
                {
                    currentTodo.Description = todo.Description;
                    currentTodo.Done = todo.Done;
                }
            }
            return todo;
        }

        #endregion

        private readonly List<Todo> Todos;
        private readonly ITodoRepository Repository;
    }
}
