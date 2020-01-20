using Google.Protobuf.WellKnownTypes;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.Grpc
{
    public class GrpcDataProvider : GenFuTodoDataProvider
    {
        static GrpcDataProvider()
        {
            TodoMessages = new List<TodoMessage>(Todos.Select(t =>
            {
                return new TodoMessage()
                {
                    Boolean1 = t.Boolean1,
                    Date1 = Timestamp.FromDateTime(t.Date1),
                    Date2 = Timestamp.FromDateTimeOffset(t.Date2),
                    Description = t.Description,
                    Done = t.Done,
                    Id = t.Id,
                    Number1 = t.Number1,
                    Number2 = t.Number2,
                    Number3 = t.Number3,
                    Number4 = Convert.ToDouble(t.Number4),
                    String1 = t.String1,
                    String2 = t.String2,
                    UserId = t.UserId
                };
            })).ToList();
        }


        public Task<TodoMessage> First()
        {
            return Task.FromResult(TodoMessages.FirstOrDefault());
        }

        public new Task<IEnumerable<TodoMessage>> Get()
        {
            return Task.FromResult(TodoMessages.AsEnumerable());
        }

        public Task<bool> Send(IEnumerable<TodoMessage> messages)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SendOne(TodoMessage message)
        {
            return Task.FromResult(true);
        }


        private readonly static List<TodoMessage> TodoMessages;
    }
}
