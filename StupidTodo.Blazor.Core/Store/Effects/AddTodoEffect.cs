using Fluxor;
using StupidTodo.Blazor.Core.Store.Actions;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Blazor.Core.Store.Effects
{
    public class AddTodoEffect
    {
        private readonly ITodoApi _api;

        public AddTodoEffect(ITodoApi api) =>
            _api = api;

        [EffectMethod]
        public async Task HandleAddTodoAction(AddTodoAction action, IDispatcher dispatcher)
        {
            var todo = await _api.AddTodoAsync(new Todo(action.Description, action.UserId));

            AddTodoResultAction resultAction;
            if (todo != null) { resultAction = new AddTodoResultAction(todo); }
            else { resultAction = new AddTodoResultAction(new Todo(), $"Failed to add todo \"{action.Description}\""); }

            dispatcher.Dispatch(resultAction);
        }
    }
}
