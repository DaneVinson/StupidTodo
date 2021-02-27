using Fluxor;
using StupidTodo.Blazor.Core.Store.States;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Features
{
    public class TodosFeature : Feature<TodosState>
    {
        public override string GetName() => "Todos";

        protected override TodosState GetInitialState()
        {
            return new TodosState(
                            todos: Array.Empty<ITodo>(),
                            busy: false,
                            showingDone: false);
        }
    }
}
