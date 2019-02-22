using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StupidTodo.AzureStorageTables;
using StupidTodo.Domain;

namespace StupidTodo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        public TodoController(ITodoRepository repository, UserOptions user)
        {
            Repository = repository ?? throw new ArgumentNullException();
            TodoUser = user ?? throw new ArgumentNullException();
        }


        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodoAsync([FromBody]Todo todo)
        {
            todo.UserId = TodoUser.Id;
            todo = await Repository.AddTodoAsync(todo);
            if (todo == null) { return BadRequest(); }
            return todo;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteTodoAsync(string id)
        {
            if (await Repository.DeleteTodoAsync(TodoUser.Id, id)) { return Ok(); }
            else { return BadRequest(); }
        }

        [HttpGet]
        [Route("done")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetDoneTodosAsync()
        {
            return Ok(await Repository.GetTodosAsync(TodoUser.Id, true));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
        {
            return Ok(await Repository.GetTodosAsync(TodoUser.Id));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            todo = await Repository.UpdateTodoAsync(todo);
            if (todo == null) { return BadRequest(); }
            else { return todo; }
        }


        private readonly ITodoRepository Repository;
        private readonly UserOptions TodoUser;
    }
}