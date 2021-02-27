using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Actions
{
    public class DeleteTodoAction
    {
        public DeleteTodoAction(string id) => 
            Id = id ?? string.Empty;

        public string Id { get; }
    }
}
