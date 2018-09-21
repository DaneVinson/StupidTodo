using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.Client.Blazor
{
    public class StatefulTodo : Todo
    {
        public StatefulTodo(ITodo todo) : base(todo)
        {
            DescriptionEdit = todo?.Description;
        }

        public string DescriptionEdit { get; set; }
        public bool IsEditing { get; set; }
    }
}
