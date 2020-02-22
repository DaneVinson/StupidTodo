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
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        public TodoController(ITodoDataProvider dataProvider)
        {
            DataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
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

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            if (todo.Id == null) { return BadRequest(); }
            return await DataProvider.Upsert(todo);
        }


        private readonly ITodoDataProvider DataProvider;
    }
}