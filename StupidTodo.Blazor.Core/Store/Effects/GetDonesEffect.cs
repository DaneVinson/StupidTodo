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
    public class GetDonesEffect
    {
        private readonly ITodoApi _api;

        public GetDonesEffect(ITodoApi api) =>
            _api = api;

        [EffectMethod]
        public async Task HandleGetTodosAction(GetDonesAction action, IDispatcher dispatcher)
        {
            var todos = await _api.GetTodosAsync(done: true);
            dispatcher.Dispatch(new GetDonesResultAction(todos ?? Array.Empty<ITodo>(), null));
        }
    }
}
