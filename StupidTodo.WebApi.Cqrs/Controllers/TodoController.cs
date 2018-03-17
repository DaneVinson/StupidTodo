using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StupidTodo.Domain;
using StupidTodo.Domain.EventSource;

namespace StupidTodo.WebApi.Cqrs.Controllers
{
    [Produces("application/json")]
    [Route("api/todo")]
    public class TodoController : Controller
    {
        public TodoController(IQueueWriter<ICommand> queueWriter, IProjector<Todo> projector)
        {
            CommandQueueWriter = queueWriter ?? throw new ArgumentNullException();
            Projector = projector ?? throw new ArgumentNullException();
        }

        #region REST

        [HttpPost]
        public async Task<IActionResult> AddTodoAsync([FromBody]Todo todo)
        {
            if (todo == null) { return BadRequest(); }

            return await QueueCommand(new CreateCommand()
            {
                Description = todo.Description,
                TargetId = todo.Id
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTodoAsync(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) { return BadRequest(); }

            return await QueueCommand(new DeleteCommand() { TargetId = id });
        }

        [HttpGet]
        [Route("done")]
        public async Task<IActionResult> GetDoneTodosAsync()
        {
            var result = await Projector.ViewAsync(true);
            if (result.Success) { return Ok(result.Value); }
            else { return BadRequest(result); }
        }

        [HttpGet]
        public async Task<IActionResult> GetTodosAsync()
        {
            var result = await Projector.ViewAsync(false);
            if (result.Success) { return Ok(result.Value); }
            else { return BadRequest(result); }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTodoAsync(string id, [FromBody]Todo todo)
        {
            if (todo == null || String.IsNullOrWhiteSpace(id) || !id.Equals(todo.Id, StringComparison.OrdinalIgnoreCase)) { return BadRequest(); }

            return await QueueCommand(new UpdateCommand()
            {
                Description = todo.Description,
                Done = todo.Done,
                TargetId = todo.Id
            });
        }

        #endregion

        #region Private

        private async Task<IActionResult> QueueCommand(ICommand command)
        {
            try
            {
                await CommandQueueWriter.WriteAsync(command);
                return Accepted();
            }
            catch (Exception exception)
            {
                return StatusCode(500);
            }
        }


        private readonly IQueueWriter<ICommand> CommandQueueWriter;
        private readonly IProjector<Todo> Projector;

        #endregion
    }
}