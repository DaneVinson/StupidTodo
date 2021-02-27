using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Actions
{
    public class DeleteTodoResultAction
    {
        public DeleteTodoResultAction(string id, string? error) =>
            (Id, Error) = (id, error);

        public string Id { get; }
        public string? Error { get; }
        public bool IsError => !string.IsNullOrWhiteSpace(Error);
    }
}
