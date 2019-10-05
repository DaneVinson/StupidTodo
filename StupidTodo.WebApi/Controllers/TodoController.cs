using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StupidTodo.Domain;

namespace StupidTodo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodoAsync([FromBody]Todo todo)
        {
            await Task.CompletedTask;

            return UpsertTodo(todo);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTodoAsync(string id)
        {
            await Task.CompletedTask;

            using (var database = new LiteDatabase(DatabaseName))
            {
                var success = database.GetCollection<Todo>(TodoCollectionName).Delete(id);
                if (success) { return Ok(); }
                else { return BadRequest(); }
            }
        }

        [HttpGet]
        [Route("done")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetDoneTodosAsync()
        {
            await Task.CompletedTask;

            return GetTodos(true);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
        {
            await Task.CompletedTask;

            return GetTodos(false);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            await Task.CompletedTask;

            return UpsertTodo(todo);
        }


        private Todo[] GetTodos(bool done)
        {
            using (var database = new LiteDatabase(DatabaseName))
            {
                return database.GetCollection<Todo>(TodoCollectionName)
                                .Find(t => t.Done == done)
                                .ToArray();
            }
        }

        private ActionResult<Todo> UpsertTodo(Todo todo)
        {
            // Validate
            if (todo == null || 
                String.IsNullOrWhiteSpace(todo.Description) || 
                String.IsNullOrWhiteSpace(todo.Id))
            {
                return BadRequest();
            }

            using (var database = new LiteDatabase(DatabaseName))
            {
                var result = database.GetCollection<Todo>(TodoCollectionName).Upsert(todo);
                if (result) { return todo; }
                else { return BadRequest(); }
            }
        }


        private const string DatabaseName = "strupid-todo.db";
        private const string TodoCollectionName = "todos";
    }
}