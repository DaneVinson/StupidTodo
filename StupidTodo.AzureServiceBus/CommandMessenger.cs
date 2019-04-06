using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.AzureServiceBus
{
    public class CommandMessenger : IMessenger<CommandMessage>
    {
        public CommandMessenger(IQueueClient queueClient)
        {
            QueueClient = queueClient ?? throw new ArgumentNullException();
        }


        public Task SendAsync(CommandMessage message)
        {
            if (message == null) { return Task.CompletedTask; }
            return QueueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));
        }


        private readonly IQueueClient QueueClient;
    }
}
