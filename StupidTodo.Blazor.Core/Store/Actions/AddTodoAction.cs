using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Actions
{
    public class AddTodoAction
    {
        public AddTodoAction(string description, string userId)
        {
            Description = description;
            UserId = userId;
        }

        public string Description { get; }
        public string UserId { get; }
    }
}
