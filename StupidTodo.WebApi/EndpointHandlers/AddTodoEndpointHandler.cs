namespace StupidTodo.WebApi.EndpointHandlers;

public class AddTodoEndpointHandler
{
    private readonly IMediator _mediator;

    public AddTodoEndpointHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IResult> HandleAsync(Todo todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Description))
        {
            return Results.BadRequest(); 
        }

        var resultTodo = await _mediator.Send(new AddEntity<Todo>(todo));
        
        return Results.Ok(resultTodo);
    }
}