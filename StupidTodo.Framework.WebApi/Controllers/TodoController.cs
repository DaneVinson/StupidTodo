using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using StupidTodo.Domain;

namespace StupidTodo.Framework.WebApi.Controllers
{
    [RoutePrefix("api/todo")]
    public class TodoController : ApiController
    {
        public TodoController()
        {
            DataProvider = new GenFuTodoDataProvider();
        }


        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAllTodosAsync()
        {
            return Ok((await DataProvider.Get()).ToArray());
        }

        [HttpGet]
        [Route("first")]
        public async Task<IHttpActionResult> GetFirstTodosAsync()
        {
            return Ok(await DataProvider.GetFirst());
        }

        [HttpPost]
        [Route("send")]
        public async Task<IHttpActionResult> SendAsync(IEnumerable<Todo> todos)
        {
            return Ok(await DataProvider.Send(todos));
        }

        [HttpPost]
        [Route("send-one")]
        public async Task<IHttpActionResult> SendOneAsync(Todo todo)
        {
            return Ok(await DataProvider.Send(todo));
        }


        private readonly IServiceCompareDataProvider DataProvider;
    }
}