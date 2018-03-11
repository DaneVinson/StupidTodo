using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IDispatcher<T>
    {
        Task ReceiveAndExecuteAsync(string message);
    }

    public class CommandDispatcher<TEntity> : IDispatcher<ICommand>
    {
        public CommandDispatcher(
            IEventStore eventStore,
            IProjector<TEntity> projector)
        {
            EventStore = eventStore ?? throw new ArgumentNullException();
            Projector = projector ?? throw new ArgumentNullException();
        }

        public async Task ReceiveAndExecuteAsync(string message)
        {
            var jObject = JObject.Parse(message);
            var handlerType = jObject["HandlerType"]?.ToString();
            var handler = Activator.CreateInstance(Type.GetType(handlerType), EventStore, Projector) as ICommandHandler;
            await handler.ExecuteCommandAsync(message);
        }

        protected readonly IEventStore EventStore;
        protected readonly IProjector<TEntity> Projector;
    }
}
