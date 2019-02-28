using Orleans;
using Orleans.Configuration;
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
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        public TodoController(ITodoRepository repository, IClusterClient clusterClient, UserOptions user)
        {
            ClusterClient = clusterClient ?? throw new ArgumentNullException();
            Repository = repository ?? throw new ArgumentNullException();
            TodoUser = user ?? throw new ArgumentNullException();
        }


        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodoAsync([FromBody]Todo todo)
        {
            todo = await GetGrain().AddTodoAsync(todo);
            if (todo == null) { return BadRequest(); }
            return todo;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteTodoAsync(string id)
        {
            var success = await GetGrain().DeleteTodoAsync(id);
            if (success) { return Ok(); }
            return BadRequest();
        }

        [HttpGet]
        [Route("done")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetDoneTodosAsync()
        {
            return (await GetGrain().GetTodosAsync(true))?.ToArray();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
        {
            return (await GetGrain().GetTodosAsync())?.ToArray();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            todo = await GetGrain().UpdateTodoAsync(todo);
            if (todo == null) { return BadRequest(); }
            return todo;
        }


        private IUserTodosGrain GetGrain()
        {
            return ClusterClient.GetGrain<IUserTodosGrain>(TodoUser.Id);
        }


        private readonly IClusterClient ClusterClient;
        private readonly ITodoRepository Repository;
        private readonly UserOptions TodoUser;
    }
}