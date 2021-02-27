using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Actions
{
    public class UpdateTodoAction
    {
        public UpdateTodoAction(string id, string description, bool done) =>
            (Description, Done, Id) = (description, done, id);

        public string Description { get; }
        public bool Done { get; }
        public string Id { get; }
    }
}
