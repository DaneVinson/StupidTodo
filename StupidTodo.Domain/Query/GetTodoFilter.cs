using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain.Query
{
    public class GetTodoFilter
    {
        public GetTodoFilter()
        {
            Id = string.Empty;
        }

        public GetTodoFilter(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}
