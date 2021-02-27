using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.States
{
    public class TodoState : ITodo
    {
        public TodoState(ITodo todo) : this(todo.Description, todo.Done, todo.Id, todo.UserId)
        { }
 
        public TodoState(string description = "", bool done = false, string id  = "", string userId = "") =>
            (Description, Done, Id, UserId) = (description, done, id, userId);

        public string Description { get; }
        public bool Done { get; }
        public string Id { get; }
        public string UserId { get; }
    }
}
