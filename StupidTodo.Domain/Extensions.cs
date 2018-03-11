using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StupidTodo.Domain
{
    public static class Extensions
    {
        public static Todo GetTodo(this ITodo todo)
        {
            return new Todo()
            {
                Description = todo.Description,
                Done = todo.Done,
                Id = todo.Id
            };
        }

        public static bool IsSuccessCode(this HttpStatusCode code)
        {
            return ((int)code).IsSuccessCode();
        }

        public static bool IsSuccessCode(this int code)
        {
            return code > 199 && code < 300;
        }
    }
}
