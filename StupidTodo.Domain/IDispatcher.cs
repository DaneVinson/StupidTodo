using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface IDispatcher<T>
    {
        Task<Result> DispatchAsync(string message);
    }
}
