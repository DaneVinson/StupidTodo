using StupidTodo.Blazor.Core.Store.States;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Actions
{
    public class AddTodoResultAction
    {
        public AddTodoResultAction(ITodo todo, string? error = null)
        {
            Error = error;
            if (Error == null) { Todo = new TodoState(todo); }
            else { Todo = new TodoState(); }
        }

        public string? Error { get; }
        public TodoState Todo { get; }

    }
}
