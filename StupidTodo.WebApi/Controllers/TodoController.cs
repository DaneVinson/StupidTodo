using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StupidTodo.Domain;
using Newtonsoft.Json;

namespace StupidTodo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        public TodoController(
            IClusterClient clusterClient, 
            IMessenger<CommandMessage> commandMessenger, 
            UserOptions user)
        {
            ClusterClient = clusterClient ?? throw new ArgumentNullException();
            CommandMessenger = commandMessenger ?? throw new ArgumentNullException();
            TodoUser = user ?? throw new ArgumentNullException();
        }


        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodoAsync([FromBody]Todo todo)
        {
            todo.UserId = TodoUser.Id;
            todo.Id = Guid.NewGuid().ToString();
            await CommandMessenger.SendAsync(new CommandMessage(
                                                    todo.UserId,
                                                    nameof(CreateTodoCommand),
                                                    JsonConvert.SerializeObject(new CreateTodoCommand(todo.Id, todo.Description))));
            return Accepted(todo);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteTodoAsync(string id)
        {
            await CommandMessenger.SendAsync(new CommandMessage(
                                                    TodoUser.Id,
                                                    nameof(DeleteTodoCommand),
                                                    JsonConvert.SerializeObject(new DeleteTodoCommand(id))));
            return Accepted();
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
            todo.UserId = TodoUser.Id;
            await CommandMessenger.SendAsync(new CommandMessage(
                                                    todo.UserId,
                                                    nameof(UpdateTodoCommand),
                                                    JsonConvert.SerializeObject(new UpdateTodoCommand(todo.Id, todo.Description, todo.Done))));
            return Accepted(todo);
        }


        private IUserTodosGrain GetGrain()
        {
            if (!ClusterClient.IsInitialized) { ClusterClient.Connect().GetAwaiter().GetResult(); }
            return ClusterClient.GetGrain<IUserTodosGrain>(TodoUser.Id);
        }


        private readonly IClusterClient ClusterClient;
        private readonly IMessenger<CommandMessage> CommandMessenger;
        private readonly UserOptions TodoUser;
    }
}