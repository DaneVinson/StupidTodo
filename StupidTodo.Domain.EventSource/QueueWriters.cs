using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public interface IQueueWriter<TMessage>
    {
        Task WriteAsync(TMessage message);
    }

    public class CommandQueueWriter : IQueueWriter<ICommand>
    {
        public CommandQueueWriter(IDispatcher<ICommand> dispatcher)
        {
            // TODO: move dispatcher (queue listener) to continuous proecess.
            CommandDispatcher = dispatcher ?? throw new ArgumentNullException();
        }


        public async Task WriteAsync(ICommand message)
        {
            var json = JsonConvert.SerializeObject(message);

            // TODO: remove when dispatcher is moved
            await CommandDispatcher.DispatchAsync(json);
        }


        private readonly IDispatcher<ICommand> CommandDispatcher;
    }
}
