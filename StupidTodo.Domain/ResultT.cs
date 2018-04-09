using System;
using System.Collections.Generic;
using System.Text;

namespace StupidTodo.Domain
{
    /// <summary>
    /// Simple generic result object.
    /// </summary>
    public class Result<T> : Result
    {
        public Result() : base()
        { }

        public Result(string message) : base(message)
        { }

        public Result(bool success) : base(success)
        { }


        public T Value { get; set; }
    }
}
