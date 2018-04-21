using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public class CommandDispatcher : IDispatcher<ICommand>
    {
        public CommandDispatcher(ICommandHandler commandHandler)
        {
            CommandHandler = commandHandler ?? throw new ArgumentNullException();
        }


        public async Task<Result> DispatchAsync(string message)
        {
            var commandType = JObject.Parse(message)?["CommandType"]?.ToString();
            if (String.IsNullOrWhiteSpace(commandType)) { return new Result("The message was not a command."); }
            var type = Type.GetType(commandType);
            if (type == null) { return new Result("The message was not a known type."); }

            var command = JsonConvert.DeserializeObject(message, type) as ICommand;
            return await CommandHandler.ExecuteAsync(command);
        }


        private readonly ICommandHandler CommandHandler;
    }
}
