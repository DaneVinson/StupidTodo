using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StupidTodo.Domain;

namespace StupidTodo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        public TodoController(IServiceCompareDataProvider dataProvider)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }


        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetAllTodosAsync()
        {
            return (await DataProvider.Get()).ToArray();
        }

        [HttpGet]
        [Route("first")]
        public async Task<ActionResult<Todo>> GetFirstTodosAsync()
        {
            return (await DataProvider.GetFirst());
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult<bool>> SendAsync(IEnumerable<Todo> todos)
        {
            return (await DataProvider.Send(todos));
        }

        [HttpPost]
        [Route("send-one")]
        public async Task<ActionResult<bool>> SendOneAsync(Todo todo)
        {
            return (await DataProvider.Send(todo));
        }


        private readonly IServiceCompareDataProvider DataProvider;


        #region Default Stupid Todo REST

        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodoAsync([FromBody]Todo todo)
        {
            if (todo?.Description == null) { return BadRequest(); }

            return await DataProvider.Upsert(todo);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteTodoAsync(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) { return BadRequest(); }

            var success = await DataProvider.Delete(id);
            if (success) { return Ok(); }
            return NotFound();
        }

        [HttpGet]
        [Route("done")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetDoneTodosAsync()
        {
            return (await DataProvider.Get(true)).ToArray();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
        {
            return (await DataProvider.Get(false)).ToArray();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            if (todo.Id == null) { return BadRequest(); }
            return await DataProvider.Upsert(todo);
        }

        #endregion
    }
}