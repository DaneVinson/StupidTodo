using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class Todo : ITodo
    {
        public Todo()
        { }

        public Todo(ITodo todo)
        {
            if (todo != null)
            {
                Description = todo.Description;
                Done = todo.Done;
                Id = todo.Id;
            }
        }


        public string Description { get; set; }
        public bool Done { get; set; }
        public string Id { get; set; }
    }
}
