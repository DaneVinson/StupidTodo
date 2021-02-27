using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Actions
{
    public class UpdateTodoResultAction
    {
        public UpdateTodoResultAction(ITodo? todo, string? error) =>
            (Todo, Error) = (todo, error);

        public string? Error { get; }
        public ITodo? Todo { get; }
    }
}
