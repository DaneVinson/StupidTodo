using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StupidTodo.Domain;

namespace StupidTodo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/todos")]
    public class TodosController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> AddTodoAsync([FromBody]Todo todo)
        {
            await Task.CompletedTask;
            if (todo?.Description == null || ToDos.Any(t => t.Id == todo.Id)) { return BadRequest(); }
            else
            {
                ToDos.Insert(0, todo);
                return Ok();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTodoAsync(string id)
        {
            await Task.CompletedTask;
            var todo = ToDos.FirstOrDefault(t => t.Id == id);
            if (todo == null) { return NotFound(); }
            else
            {
                ToDos.Remove(todo);
                return Ok();
            }
        }

        [HttpGet]
        [Route("done")]
        public async Task<IActionResult> GetDoneTodosAsync()
        {
            await Task.CompletedTask;
            return Ok(ToDos.Where(t => t.Done).ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> GetTodosAsync()
        {
            await Task.CompletedTask;
            return Ok(ToDos.Where(t => !t.Done).ToArray());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            await Task.CompletedTask;
            var existingTodo = ToDos.FirstOrDefault(t => t.Id == id);
            if (existingTodo == null) { return NotFound(); }
            else if (String.IsNullOrWhiteSpace(todo.Description)) { return BadRequest(); }
            else
            {
                existingTodo.Description = todo.Description;
                existingTodo.Done = todo.Done;
                return Ok();
            }
        }


        static TodosController()
        {
            ToDos = new List<Todo>()
            {
                new Todo() { Description = "Gas up the car", Id = Guid.NewGuid().ToString() },
                new Todo() { Description = "Find my next book", Id = Guid.NewGuid().ToString() },
                new Todo() { Description = "Pick up milk", Id = Guid.NewGuid().ToString() },
                new Todo() { Description = "Take a breath", Done = true, Id = Guid.NewGuid().ToString() }
            };
        }

        private static readonly List<Todo> ToDos;
    }
}