using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Actions
{
    public class GetDonesResultAction
    {
        public GetDonesResultAction(IEnumerable<ITodo> todos, string? error) =>
            (Error, Todos) = (error, todos ?? Array.Empty<ITodo>());

        public string? Error { get; }
        public IEnumerable<ITodo> Todos { get; }
    }
}
