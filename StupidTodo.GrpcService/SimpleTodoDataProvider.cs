using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.GrpcService
{
    public class SimpleTodoDataProvider : ITodoDataProvider
    {
        public Task<bool> Delete(string id)
        {
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo != null) { Todos.Remove(todo); }
            return Task.FromResult(todo != null);
        }

        public Task<IEnumerable<Todo>> Get(bool done = false)
        {
            return Task.FromResult(Todos.Where(t => t.Done == done));
        }

        public Task<Todo> Upsert(Todo upsertTodo)
        {
            var todo = Todos.FirstOrDefault(t => t.Id == upsertTodo.Id);
            if (todo == null)
            {
                todo = upsertTodo;
                Todos.Add(todo);
            }
            else
            {
                todo.Description = upsertTodo.Description;
                todo.Done = upsertTodo.Done;
            }
            return Task.FromResult(todo);
        }


        static SimpleTodoDataProvider()
        {
            Todos = new List<Todo>()
            {
                new Todo() { Description = "Gas up the car", Id = Guid.NewGuid().ToString(), UserId = Bilbo.Id },
                new Todo() { Description = "Find my next book", Id = Guid.NewGuid().ToString(), UserId = Bilbo.Id },
                new Todo() { Description = "Pick up milk", Id = Guid.NewGuid().ToString(), UserId = Bilbo.Id },
                new Todo() { Description = "Take a breath", Done = true, Id = Guid.NewGuid().ToString(), UserId = Bilbo.Id }
            };
        }

        private static readonly List<Todo> Todos;
    }
}
