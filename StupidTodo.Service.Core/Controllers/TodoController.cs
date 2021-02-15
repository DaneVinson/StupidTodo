using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StupidTodo.Domain;

namespace StupidTodo.Service.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase, ITodoApi
    {
        private readonly ITodoDataProvider _dataProvider;

        public TodoController(ITodoDataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }


        public async Task<Todo?> AddTodoAsync(Todo todo)
        {
            if (todo?.Description == null) { return null; }
            todo.Id = Guid.NewGuid().ToString();

            return await _dataProvider.Upsert(todo);
        }

        public async Task<bool> DeleteTodoAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { return false; }

            return await _dataProvider.Delete(id);
        }

        public async Task<Todo[]?> GetTodosAsync(bool done = false)
        {
            return (await _dataProvider.Get(done)).ToArray();
        }

        public async Task<Todo?> UpdateTodoAsync(Todo todo)
        {
            return await _dataProvider.Upsert(todo);
        }


        [HttpPost]
        public async Task<ActionResult<Todo>> AddAsync([FromBody]Todo todo)
        {
            var resultTodo = await AddTodoAsync(todo);
            if (resultTodo == null) { return BadRequest(); }

            return resultTodo;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var success = await DeleteTodoAsync(id);
            if (success) { return Ok(); }
            else { return NotFound(); }
        }

        [HttpGet]
        [Route("done")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetDoneAsync()
        {
            return await GetTodosAsync(true) ?? Array.Empty<Todo>();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetAsync()
        {
            return await GetTodosAsync(false) ?? Array.Empty<Todo>();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateAsync(string id, [FromBody]Todo todo)
        {
            if (id == null || 
                todo?.Id == null || 
                !id.Equals(todo.Id, StringComparison.OrdinalIgnoreCase)) 
            { return BadRequest(); }

            var resultTodo = await UpdateTodoAsync(todo);
            if (resultTodo == null) { return BadRequest(); }

            return resultTodo;
        }
    }
}