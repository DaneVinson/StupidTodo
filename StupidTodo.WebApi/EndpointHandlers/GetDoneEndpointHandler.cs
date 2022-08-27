namespace StupidTodo.WebApi.EndpointHandlers;

public class GetDoneEndpointHandler
{
    private readonly IMediator _mediator;

    public GetDoneEndpointHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IResult> HandleAsync()
    {
        var todos = await _mediator.Send(new GetEntitiesQuery<Todo, GetDoneFilter>());
        return Results.Ok(todos.ToArray());
    }
}