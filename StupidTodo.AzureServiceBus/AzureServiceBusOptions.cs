using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.AzureServiceBus
{
    public class AzureServiceBusOptions
    {
        public AzureServiceBusOptions()
        { }

        public AzureServiceBusOptions(string connectionString, string commandQueueName)
        {
            CommandQueueName = commandQueueName;
            ConnectionString = connectionString;
        }


        public string CommandQueueName { get; set; }
        public string ConnectionString { get; set; }
    }
}
