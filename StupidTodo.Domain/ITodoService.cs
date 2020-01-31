using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    [ServiceContract(Namespace = "http://todoservice.com")]
    public interface ITodoService
    {
        [OperationContract]
        Task<Todo> First();

        [OperationContract]
        Task<IEnumerable<Todo>> Get();

        [OperationContract]
        Task<bool> Send(IEnumerable<Todo> todos);

        [OperationContract]
        Task<bool> SendOne(Todo todo);
    }
}
