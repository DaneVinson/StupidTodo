using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public interface IMessenger<TMessage>
    {
        Task SendAsync(TMessage message);
    }
}
