using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    public class Result
    {
        public string Message { get; set; }
        public bool Success { get; set; }

        public override string ToString()
        {
            return Success ? "Success" : $"Failure: {Message}";
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }
    }
}
