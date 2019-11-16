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
        public TodoController(ITodoDataProvider dataProvider, StupidTodoOptions options)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

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
            return (await DataProvider.Get()).ToArray();
        }

        [HttpGet]
        [Route("options")]
        public ActionResult<StupidTodoOptions> GetOptions()
        {
            return Options;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            if (todo.Id == null) { return BadRequest(); }
            return await DataProvider.Upsert(todo);
        }


        private readonly ITodoDataProvider DataProvider;
        private readonly StupidTodoOptions Options;
    }
}