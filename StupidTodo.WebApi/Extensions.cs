namespace StupidTodo.WebApi;

public static class Extensions
{
    public static WebApplication MapTodoEndpoints(this WebApplication app)
    {
        app.MapGet(
            "api/todo",
            async (ITodoApi api) =>
                Results.Ok(await api.GetTodosAsync()));
        app.MapGet(
            "api/todo/done",
            async (ITodoApi api) =>
                Results.Ok(await api.GetTodosAsync(true)));
        app.MapPost(
            "api/todo",
            async (ITodoApi api, [FromBody] Todo todo) =>
            { 
                if (string.IsNullOrWhiteSpace(todo?.Description)) 
                {
                    return Results.BadRequest(); 
                }

                return Results.Ok(await api.AddTodoAsync(todo)); 
            });
        app.MapPut(
            "api/todo/{id}",
            async (ITodoApi api, string id, [FromBody] Todo todo) =>
            {
                if (string.IsNullOrWhiteSpace(id)) 
                {
                    return Results.BadRequest();
                }

                return Results.Ok(await api.UpdateTodoAsync(todo));
            });
        app.MapDelete(
            "api/todo/{id}",
            async (ITodoApi api, string id) =>
            {
                if (string.IsNullOrWhiteSpace(id)) 
                {
                    return Results.BadRequest(); 
                }

                if (await api.DeleteTodoAsync(id))
                {
                    return Results.Ok();
                }

                return Results.NotFound();
            });

        return app;
    }
}
