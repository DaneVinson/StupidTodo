using StupidTodo.WebApi.EndpointHandlers;

namespace StupidTodo.WebApi;

public static class Extensions
{
    public static WebApplication MapTodoEndpoints2(this WebApplication app)
    {
        app.MapGet(
            "api/todo", 
            async (GetTodosEndpointHandler handler) => 
                await handler.HandleAsync());
        app.MapGet(
            "api/todo/done", 
            async (GetDoneEndpointHandler handler) => 
                await handler.HandleAsync());
        app.MapPost(
            "api/todo",
            async (AddTodoEndpointHandler handler, [FromBody] Todo todo) =>
                await handler.HandleAsync(todo));
        app.MapPut(
            "api/todo/{id}",
            async (UpdateTodoEndpointHandler handler, string id, [FromBody] Todo todo) =>
                await handler.HandleAsync(todo));
        app.MapDelete(
            "api/todo/{id}",
            async (DeleteTodoEndpointHandler handler, string id) =>
                await handler.HandleAsync(id));

        return app;
    }
    
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
                Results.Ok(await dataProvider.Upsert(todo)));
        app.MapPut(
            "api/todo/{id}",
            async (ITodoDataProvider dataProvider, string id, [FromBody] Todo todo) =>
                Results.Ok(await dataProvider.Upsert(todo)));
        app.MapDelete(
            "api/todo/{id}",
            async (ITodoDataProvider dataProvider, string id) =>
            {
                if (await dataProvider.Delete(id))
                {
                    return Results.Ok();
                }

                return Results.NotFound();
            });

        return app;
    }
}