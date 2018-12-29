using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StupidTodo.Domain;

namespace StupidTodo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        public TodoController(
            TheBeer beer,
            IOptions<TheBeer> beerOptions, 
            IOptions<List<Todo>> todoOptions)
        {
            Beer = beer;
            BeerFromOptions = beerOptions?.Value;
            Todos = todoOptions?.Value;
        }


        [HttpPost]
        public async Task<IActionResult> AddTodoAsync([FromBody]Todo todo)
        {
            await Task.CompletedTask;
            if (todo?.Description == null || Todos.Any(t => t.Id == todo.Id)) { return BadRequest(); }
            else
            {
                Todos.Insert(0, todo);
                return Ok(todo);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTodoAsync(string id)
        {
            await Task.CompletedTask;
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null) { return NotFound(); }
            else
            {
                Todos.Remove(todo);
                return Ok();
            }
        }

        [HttpGet]
        [Route("done")]
        public async Task<IActionResult> GetDoneTodosAsync()
        {
            await Task.CompletedTask;
            return Ok(Todos.Where(t => t.Done).ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> GetTodosAsync()
        {
            await Task.CompletedTask;
            return Ok(Todos.Where(t => !t.Done).ToArray());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            await Task.CompletedTask;
            var existingTodo = Todos.FirstOrDefault(t => t.Id == id);
            if (existingTodo == null) { return NotFound(); }
            else if (String.IsNullOrWhiteSpace(todo.Description)) { return BadRequest(); }
            else
            {
                existingTodo.Description = todo.Description;
                existingTodo.Done = todo.Done;
                return Ok(existingTodo);
            }
        }


        private readonly TheBeer BeerFromOptions;
        private readonly TheBeer Beer;
        private readonly List<Todo> Todos;

        #region Static Todos, deprecated for branch

        //static TodoController()
        //{
        //    Todos = new List<Todo>()
        //    {
        //        new Todo() { Description = "Gas up the car", Id = Guid.NewGuid().ToString() },
        //        new Todo() { Description = "Find my next book", Id = Guid.NewGuid().ToString() },
        //        new Todo() { Description = "Pick up milk", Id = Guid.NewGuid().ToString() },
        //        new Todo() { Description = "Take a breath", Done = true, Id = Guid.NewGuid().ToString() }
        //    };
        //}

        //private static readonly List<Todo> Todos;

        #endregion
    }
}