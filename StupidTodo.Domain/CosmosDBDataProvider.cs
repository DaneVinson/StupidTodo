using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public class CosmosDBDataProvider : ITodoDataProvider
    {
        private readonly TodosDbContext _context;

        public CosmosDBDataProvider(TodosDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Delete(string id)
        {
            // The EF way to delete with only and Id value
            var deleteTodo = new Todo() { Id = id };
            _context.Todos.Attach(deleteTodo);
            _context.Todos.Remove(deleteTodo);
            var count = await _context.SaveChangesAsync();

            return count > 0;
        }

        public async Task<IEnumerable<Todo>> Get(bool done = false)
        {
            return await _context
                            .Todos
                            .Where(t => t.Done == done)
                            .ToArrayAsync();
        }

        public async Task<Todo> Upsert(Todo todo)
        {
            var current = await _context
                                    .Todos
                                    .FirstOrDefaultAsync(t => t.Id == todo.Id);
            if (current == null)
            {
                _context.Add(todo);
                current = todo;
            }
            else 
            {
                current.Description = todo.Description;
                current.Done = todo.Done;
            }

            await _context.SaveChangesAsync();

            return current;
        }
    }
}
