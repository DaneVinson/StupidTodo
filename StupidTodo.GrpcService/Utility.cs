using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.GrpcService
{
    public static class Utility
    {
        // gRPC <-> Domain mapping functions
        public static Todo TodoFromTodoMessage(TodoMessage message)
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
        public static TodoMessage TodoMessageFromTodo(Todo todo)
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
    }
}
