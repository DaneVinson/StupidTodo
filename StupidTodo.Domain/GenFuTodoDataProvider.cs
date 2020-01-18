using GenFu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public class GenFuTodoDataProvider : SimpleTodoDataProvider, IServiceCompareDataProvider
    {
        static GenFuTodoDataProvider()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "genfu-data.json");
            if (File.Exists(path))
            {
                string json = null;
                using (var reader = new StreamReader(path))
                {
                    json = reader.ReadToEnd();
                }

                Todos = JsonSerializer.Deserialize<Todo[]>(json)?.ToList();
            }

            if (Todos == null)
            {
                GenFu.GenFu
                    .Configure<Todo>()
                    .Fill(t => t.Id, () => Guid.NewGuid().ToString())
                    .Fill(t => t.UserId, () => Bilbo.Id);

                Todos = A.ListOf<Todo>(5);
            }
        }


        public Task<IEnumerable<Todo>> Get()
        {
            return Task.FromResult(Todos.AsEnumerable());
        }

        public Task<Todo> GetFirst()
        {
            return Task.FromResult(Todos.FirstOrDefault());
        }

        public Task<bool> Send(IEnumerable<Todo> todos)
        {
            // Send received.
            return Task.FromResult(true);
        }

        public Task<bool> Send(Todo todo)
        {
            // Send received.
            return Task.FromResult(true);
        }
    }
}
