using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    /// <summary>
    /// Implementation of <see cref="IMessenger{TMessage}"/> that forwards messages to the configured
    /// <see cref="IDispatcher{T}"/> foregoing the use of a message bus.
    /// </summary>
    public class MessengerDispatcher<T> : IMessenger<T>
    {
        public MessengerDispatcher(IDispatcher<T> dispatcher)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException();
        }

        public async Task<Result> SendMessageAsync(T message)
        {
            var json = JsonConvert.SerializeObject(message);

            // Skip use of a bus and dispatch the message
            return await Dispatcher.DispatchAsync(json);
        }

        private readonly IDispatcher<T> Dispatcher;
    }
}
