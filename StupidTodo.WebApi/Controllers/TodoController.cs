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

            using (var database = NewLiteDatabase())
            {
                var success = GetTodosCollection(database).Delete(id);
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
            using (var database = NewLiteDatabase())
            {
                return GetTodosCollection(database)?.FindAll()
                                                    .Where(t => t.Done == done)
                                                    .ToArray();
            }
        }

        private LiteCollection<Todo> GetTodosCollection(LiteDatabase database)
        {
            return database.GetCollection<Todo>("todos");
        }

        private LiteDatabase NewLiteDatabase()
        {
            return new LiteDatabase("stupid-todo.db");
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

            using (var database = NewLiteDatabase())
            {
                var result = GetTodosCollection(database)?.Upsert(todo);
                if (result == null || !result.Value) { return BadRequest(); }
                else { return todo; }
            }
        }
    }
}