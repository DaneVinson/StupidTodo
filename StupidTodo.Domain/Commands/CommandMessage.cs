using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class CommandMessage
    {
        public CommandMessage()
        { }

        public CommandMessage(string partitionKey, string commandType, string commandData)
        {
            CommandData = commandData;
            CommandType = commandType;
            PartitionKey = partitionKey;
        }


        public string CommandData { get; set; }
        public string CommandType { get; set; }
        public string PartitionKey { get; set; }
    }
}
