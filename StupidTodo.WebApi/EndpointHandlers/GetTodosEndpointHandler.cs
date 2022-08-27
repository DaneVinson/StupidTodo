namespace StupidTodo.WebApi.EndpointHandlers;

public class GetTodosEndpointHandler
{
    private readonly IMediator _mediator;

    public GetTodosEndpointHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IResult> HandleAsync()
    {
        var todos = await _mediator.Send(new GetEntitiesQuery<Todo, GetTodosFilter>());
        return Results.Ok(todos.ToArray());
    }
}