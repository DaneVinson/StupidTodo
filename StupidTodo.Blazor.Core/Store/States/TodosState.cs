using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.States
{
    public class TodosState
    {
        public TodosState(IEnumerable<ITodo> todos, bool busy = false, bool showingDone = false)
        {
            Busy = busy;
            ShowDoneLabel = showingDone ? "Hide Done" : "Show Done";
            ShowingDone = showingDone;
            Todos = todos ?? Array.Empty<ITodo>();
        }

        public bool Busy { get; }
        public string ShowDoneLabel { get; }
        public bool ShowingDone { get; }
        public IEnumerable<ITodo> Todos { get; }
    }
}
