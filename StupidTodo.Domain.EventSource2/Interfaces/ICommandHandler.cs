using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource2
{
    public interface ICommandHandler
    {
        Task<Result> ExecuteAsync(ICommand command);
    }
}
