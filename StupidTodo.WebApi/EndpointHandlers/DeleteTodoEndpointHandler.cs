namespace StupidTodo.WebApi.EndpointHandlers;

public class DeleteTodoEndpointHandler
{
    private readonly IMediator _mediator;

    public DeleteTodoEndpointHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IResult> HandleAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Results.BadRequest();
        }

        var success = await _mediator.Send(new DeleteEntity<Todo>(id));

        return success ? Results.Accepted() : Results.BadRequest();
    }
}