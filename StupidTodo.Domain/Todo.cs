using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StupidTodo.Domain
{
    public class Todo : ITodo
    {
        public Todo()
        { }

        public Todo(string description, string userid) =>
            (Description, UserId) = (description, userid);

        public string Description { get; set; } = string.Empty;
        public bool Done { get; set; }
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
