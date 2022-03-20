using Microsoft.AspNetCore.Mvc;
using StupidTodo.Domain;

namespace StupidTodo.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoDataProvider _dataProvider;

    public TodoController(ITodoDataProvider dataProvider)
    {
        _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> AddTodoAsync([FromBody]Todo todo)
    {
        if (todo?.Description == null) { return BadRequest(); }

        return await _dataProvider.Upsert(todo);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteTodoAsync(string id)
    {
        if (String.IsNullOrWhiteSpace(id)) { return BadRequest(); }

        var success = await _dataProvider.Delete(id);
        if (success) { return Ok(); }
        return NotFound();
    }

    [HttpGet]
    [Route("done")]
    public async Task<ActionResult<IEnumerable<Todo>>> GetDoneTodosAsync()
    {
        return (await _dataProvider.Get(true)).ToArray();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodosAsync()
    {
        return (await _dataProvider.Get()).ToArray();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult<Todo>> UpdateTodoAsync(string id, [FromBody]Todo todo)
    {
        if (todo.Id == null) { return BadRequest(); }
        return await _dataProvider.Upsert(todo);
    }
}
