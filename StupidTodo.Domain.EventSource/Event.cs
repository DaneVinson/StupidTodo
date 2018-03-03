using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain.EventSource
{
    public class Event<T>
    {
        public virtual T Execute(string jsonSchema, T entity)
        {
            throw new NotImplementedException();
        }
    }
}
