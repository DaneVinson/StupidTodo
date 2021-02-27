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
    public class GetTodosEffect
    {
        private readonly ITodoApi _api;

        public GetTodosEffect(ITodoApi api) =>
            _api = api;

        [EffectMethod]
        public async Task HandleGetTodosAction(GetTodosAction action, IDispatcher dispatcher)
        {
            var todos = await _api.GetTodosAsync(done: false);
            dispatcher.Dispatch(new GetTodosResultAction(todos ?? Array.Empty<ITodo>(), null));
        }
    }
}
