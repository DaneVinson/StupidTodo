using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
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


        public async Task<Todo> First()
        {
            return await DataProvider.GetFirst();
        }

        public async Task<IEnumerable<Todo>> Get()
        {
            return await DataProvider.Get();
        }

        public async Task<bool> Send(IEnumerable<Todo> todos)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> SendOne(Todo todo)
        {
            return await Task.FromResult(true);
        }


        static TodoService()
        {
            GenFuTodoDataProvider.LoadDataFile(ConfigurationManager.AppSettings["DataFilePath"]);
        }

        private readonly GenFuTodoDataProvider DataProvider;
    }
}