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
    public class DeleteTodoEffect
    {
        private readonly ITodoApi _api;

        public DeleteTodoEffect(ITodoApi api) =>
            _api = api;

        [EffectMethod]
        public async Task HandleDeleteTodoAction(DeleteTodoAction action, IDispatcher dispatcher)
        {
            DeleteTodoResultAction resultAction;
            if (await _api.DeleteTodoAsync(action.Id))
            {
                resultAction = new DeleteTodoResultAction(action.Id, null);
            }
            else
            {
                resultAction = new DeleteTodoResultAction(action.Id, $"Failed to delete todo {action.Id}");
            }

            dispatcher.Dispatch(resultAction);
        }
    }
}
