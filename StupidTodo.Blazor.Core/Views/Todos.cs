using Fluxor;
using Microsoft.AspNetCore.Components;
using StupidTodo.Blazor.Core.Store.Actions;
using StupidTodo.Blazor.Core.Store.States;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Views
{
    public partial class Todos
    {
        [Inject]
        private IDispatcher Dispatcher { get; set; } = default!;

        [Inject]
        private IState<TodosState> State { get; set; } = default!;


        public IEnumerable<ITodo> Dones => State.Value.Todos.Where(t => t.Done);
        public IEnumerable<ITodo> TodoItems => State.Value.Todos.Where(t => !t.Done);


        public void Add(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) { return; }

            var action = new AddTodoAction(description, Bilbo.Id);
            Dispatcher.Dispatch(action);
        }

        public void Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { return; }

            var action = new DeleteTodoAction(id);
            Dispatcher.Dispatch(action);
        }

        public void Save(string id, string description, bool done)
        {
            var todo = State.Value.Todos.FirstOrDefault(t => t.Id.Equals(id));
            if (todo == null) { return; }

            var action = new UpdateTodoAction(
                                id,
                                string.IsNullOrWhiteSpace(description) ? todo.Description : description, 
                                done);
            Dispatcher.Dispatch(action);
        }

        public void ToggleShowDone()
        {
            var action = State.Value.ShowingDone ? new HideDoneAction() : new GetDonesAction() as object;
            Dispatcher.Dispatch(action);
        }
    }
}
