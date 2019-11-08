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
            return TodoMessageFromTodo(await DataProvider.Upsert(TodoFromTodoMessage(request)));
        }

        public override async Task<SuccessMessage> DeleteTodo(IdMessage request, ServerCallContext context)
        {
            return new SuccessMessage() { Value = await DataProvider.Delete(request.Value) };
        }

        public override Task GetDoneTodos(Empty request, IServerStreamWriter<TodoMessage> responseStream, ServerCallContext context)
        {
            return base.GetDoneTodos(request, responseStream, context);
        }

        public override Task GetTodos(Empty request, IServerStreamWriter<TodoMessage> responseStream, ServerCallContext context)
        {
            return base.GetTodos(request, responseStream, context);
        }

        public override async Task<TodoMessage> UpdateTodo(TodoMessage request, ServerCallContext context)
        {
            return TodoMessageFromTodo(await DataProvider.Upsert(TodoFromTodoMessage(request)));
        }


        // gRPC <-> Domain mapping functions
        private static Todo TodoFromTodoMessage(TodoMessage message)
        {
            var msg = message ?? new TodoMessage();
            return new Todo()
            {
                Description = msg.Description,
                Done = msg.Done,
                Id = msg.Id,
                UserId = msg.UserId
            };
        }
        private static TodoMessage TodoMessageFromTodo(Todo todo)
        {
            var to = todo ?? new Todo();
            return new TodoMessage()
            {
                Description = to.Description,
                Done = to.Done,
                Id = to.Id,
                UserId = to.UserId
            };
        }


        private readonly ITodoDataProvider DataProvider;
        private readonly ILogger<TodoService> Logger;
    }
}
