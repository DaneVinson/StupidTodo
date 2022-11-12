namespace StupidTodo.WebApi;

public static class Extensions
{
    public static WebApplication MapTodoEndpoints(this WebApplication app)
    {
        app.MapGet(
            "api/todo",
            async (ITodoDataProvider dataProvider) =>
                Results.Ok(await dataProvider.Get()));
        app.MapGet(
            "api/todo/done",
            async (ITodoDataProvider dataProvider) =>
                Results.Ok(await dataProvider.Get(true)));
        app.MapPost(
            "api/todo",
            async (ITodoDataProvider dataProvider, [FromBody] Todo todo) =>
            { 
                if (string.IsNullOrWhiteSpace(todo?.Description)) 
                {
                    return Results.BadRequest(); 
                }

                return Results.Ok(await dataProvider.Upsert(todo)); 
            });
        app.MapPut(
            "api/todo/{id}",
            async (ITodoDataProvider dataProvider, string id, [FromBody] Todo todo) =>
            {
                if (string.IsNullOrWhiteSpace(id)) 
                {
                    return Results.BadRequest();
                }

                return Results.Ok(await dataProvider.Upsert(todo));
            });
        app.MapDelete(
            "api/todo/{id}",
            async (ITodoDataProvider dataProvider, string id) =>
            {
                if (string.IsNullOrWhiteSpace(id)) 
                {
                    return Results.BadRequest(); 
                }

                if (await dataProvider.Delete(id))
                {
                    return Results.Ok();
                }

                return Results.NotFound();
            });

        return app;
    }
}
