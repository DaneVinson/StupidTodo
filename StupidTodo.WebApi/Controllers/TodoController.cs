using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StupidTodo.Domain;
using StupidTodo.Domain.Command;
using StupidTodo.Domain.Query;

namespace StupidTodo.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoDataProvider _dataProvider;
        private readonly IMediator _mediator;

        public TodoController(ITodoDataProvider dataProvider, IMediator mediator)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // Command
        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodoAsync([FromBody]Todo todo)
        {
            if (todo?.Description == null) { return BadRequest(); }

            return (await _mediator.Send(new AddEntity<Todo>(todo)));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteTodoAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { return BadRequest(); }

            var _ = await _mediator.Send(new DeleteEntity<Todo>(id));

            return Accepted();
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody] Todo todo)
        {
            if (todo.Id == null || todo == null) { return BadRequest(); }

            var response = await _mediator.Send(new UpdateEntity<Todo>(todo));

            return await _dataProvider.Upsert(todo);
        }


        // Query
        [HttpGet]
        [Route("done")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetDoneTodosAsync()
        {
            return (await _mediator.Send(new GetEntitiesQuery<Todo, GetDoneFilter>())).ToArray();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
        {
            return (await _mediator.Send(new GetEntitiesQuery<Todo, GetTodosFilter>())).ToArray();
        }
    }
}