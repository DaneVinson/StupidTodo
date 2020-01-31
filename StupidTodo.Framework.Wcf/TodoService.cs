using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using System.Web;

namespace StupidTodo.Framework.Wcf
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, Namespace = "http://todoservice.com")]
    public class TodoService : ITodoService
    {
        public TodoService()
        {
            DataProvider = new GenFuTodoDataProvider();
        }


        public Task<Todo> First()
        {
            return DataProvider.GetFirst();
        }

        public Task<IEnumerable<Todo>> Get()
        {
            return DataProvider.Get();
        }

        public Task<bool> Send(IEnumerable<Todo> todos)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SendOne(Todo todo)
        {
            return Task.FromResult(true);
        }


        private readonly GenFuTodoDataProvider DataProvider;
    }
}