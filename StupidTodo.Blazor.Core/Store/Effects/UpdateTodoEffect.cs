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
    public class UpdateTodoEffect
    {
        private readonly ITodoApi _api;

        public UpdateTodoEffect(ITodoApi api) =>
            _api = api;

        [EffectMethod]
        public async Task HandleUpdateTodoAction(UpdateTodoAction action, IDispatcher dispatcher)
        {
            var todo = new Todo()
            {
                Description = action.Description,
                Done = action.Done,
                Id = action.Id
            };
            todo = await _api.UpdateTodoAsync(todo);

            UpdateTodoResultAction resultAction;
            if (todo != null) { resultAction = new UpdateTodoResultAction(todo, null); }
            else { resultAction = new UpdateTodoResultAction(null, $"Failed to update todo \"{todo.Description}\""); }

            dispatcher.Dispatch(resultAction);
        }
    }
}
