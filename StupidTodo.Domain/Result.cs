using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    /// <summary>
    /// Simple result object.
    /// </summary>
    public class Result
    {
        public Result()
        { }

        public Result(string message)
        {
            Message = message;
        }

        public Result(bool success)
        {
            Success = success;
        }

        public string Message { get; set; }
        public bool Success { get; set; }

        public override string ToString()
        {
            return Success ? "Success" : $"Failure: {Message}";
        }
    }
}
