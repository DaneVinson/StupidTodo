using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using StupidTodo.Domain;

namespace StupidTodo.GrpcService
{
    public class TodoService : TodoSvc.TodoSvcBase
    {
        public TodoService(ITodoDataProvider dataProvider, ILogger<TodoService> logger)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public override async Task<TodoMessage> AddTodo(TodoMessage request, ServerCallContext context)
        {
            return Utility.TodoMessageFromTodo(await DataProvider.Upsert(Utility.TodoFromTodoMessage(request)));
        }

        public override async Task<SuccessMessage> DeleteTodo(IdMessage request, ServerCallContext context)
        {
            return new SuccessMessage() { Value = await DataProvider.Delete(request.Value) };
        }

        public override Task GetDoneTodos(Empty request, IServerStreamWriter<TodoMessage> responseStream, ServerCallContext context)
        {
            return Get(true, responseStream);
        }

        public override Task GetTodos(Empty request, IServerStreamWriter<TodoMessage> responseStream, ServerCallContext context)
        {
            return Get(false, responseStream);
        }

        public override async Task<TodoMessage> UpdateTodo(TodoMessage request, ServerCallContext context)
        {
            return Utility.TodoMessageFromTodo(await DataProvider.Upsert(Utility.TodoFromTodoMessage(request)));
        }

        private async Task Get(bool done, IServerStreamWriter<TodoMessage> responseStream)
        {
            (await DataProvider.Get(done))
                                .Select(t => Utility.TodoMessageFromTodo(t))
                                .ToList()
                                .ForEach(t => responseStream.WriteAsync(t));
        }


        private readonly ITodoDataProvider DataProvider;
        private readonly ILogger<TodoService> Logger;
    }
}
