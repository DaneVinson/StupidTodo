using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StupidTodo.Domain;
using StupidTodo.Data.AzureTableStorage;
using StupidTodo.Domain.EventSource2;

namespace StupidTodo.WebApi.Cqrs2.Controllers
{
    [Produces("application/json")]
    [Route("api/todo")]
    public class TodoController : Controller
    {
        public TodoController(
            IProjector<ITodo, bool> projector,
            IMessenger<ICommand> commandMessenger)
        {
            CommandMessenger = commandMessenger ?? throw new ArgumentNullException();
            Projector = projector ?? throw new ArgumentNullException();
        }

        #region Command

        [HttpPost]
        public async Task<IActionResult> AddTodoAsync([FromBody]Todo todo)
        {
            if (todo == null || String.IsNullOrWhiteSpace(todo.Description)) { return BadRequest(); }
            return await SendCommandAsync(new CreateTodoCommand(todo.Description));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTodoAsync(Guid id)
        {
            if (id == Guid.Empty) { return BadRequest(); }
            return await SendCommandAsync(new DeleteTodoCommand(id));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTodoAsync(Guid id, [FromBody]Todo todo)
        {
            if (todo == null || id == Guid.Empty) { return BadRequest(); }
            return await SendCommandAsync(new UpdateTodoCommand(id, todo.Description, todo.Done));
        }

        #endregion

        #region Query

        [HttpGet]
        [Route("done")]
        public async Task<IActionResult> GetDoneTodosAsync()
        {
            var result = await Projector.ViewProjectionAsync(true);
            if (result.Success) { return Ok(result.Value); }
            else { return BadRequest(result); }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodosAsync()
        {
            var result = await Projector.ViewProjectionAsync(false);
            if (result.Success) { return Ok(result.Value); }
            else { return BadRequest(result); }
        }

        #endregion

        private async Task<IActionResult> SendCommandAsync(ICommand command)
        {
            var result = await CommandMessenger.SendMessageAsync(command);
            if (result.Success) { return Ok(); }
            else { return BadRequest(result); }
        }

        private readonly IMessenger<ICommand> CommandMessenger;
        private readonly IProjector<ITodo, bool> Projector;
    }
}