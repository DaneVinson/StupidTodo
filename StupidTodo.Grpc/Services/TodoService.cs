using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using StupidTodo.Domain;
using StupidTodo.Grpc;

namespace StupidTodo.GrpcService
{
    public class TodoService : TodoSvc.TodoSvcBase
    {
        public TodoService(GrpcDataProvider dataProvider, ILogger<TodoService> logger)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public override async Task<TodoMessage> First(EmptyMessage request, ServerCallContext context)
        {
            return await DataProvider.First();
        }

        public override async Task GetStreaming(EmptyMessage request, IServerStreamWriter<TodoMessage> responseStream, ServerCallContext context)
        {
            foreach (var todo in (await DataProvider.Get()))
            {
                await responseStream.WriteAsync(todo);
            }

            // It looks like grpc needs the returned collection to be sent linearly.
            // The following methods cause intermitent exceptions to be thrown

            //var tasks = todos.Select(t => responseStream.WriteAsync(t));
            //await Task.WhenAll(tasks);

            //(await DataProvider.Get())
            //                    .ToList()
            //                    .ForEach(t => responseStream.WriteAsync(t));
        }

        public override async Task<TodosMessage> Get(EmptyMessage request, ServerCallContext context)
        {
            var todosMessage = new TodosMessage();
            todosMessage.Todos.AddRange(await DataProvider.Get());
            return todosMessage;
        }

        public override async Task<ResultMessage> SendStreaming(IAsyncStreamReader<TodoMessage> requestStream, ServerCallContext context)
        {
            var todos = new List<TodoMessage>();
            await foreach(var todo in requestStream.ReadAllAsync())
            {
                todos.Add(todo);
            }
            return new ResultMessage() { Success = true };
        }

        public override async Task<ResultMessage> SendOne(TodoMessage request, ServerCallContext context)
        {
            var result = await DataProvider.SendOne(request);
            return new ResultMessage() { Success = result };
        }

        public override async Task<ResultMessage> Send(TodosMessage request, ServerCallContext context)
        {
            var result = await DataProvider.Send(request.Todos);
            return new ResultMessage() { Success = true };
        }

        private readonly GrpcDataProvider DataProvider;
        private readonly ILogger<TodoService> Logger;
    }
}
