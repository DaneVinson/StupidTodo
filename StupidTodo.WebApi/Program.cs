var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<ITodoDataProvider>(new SimpleTodoDataProvider())
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "StupidTodo.WebApi", Version = "v1" });
    });

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app
    .MapTodoEndpoints()
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseSwagger()
    .UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "StupidTodo.WebApi v1"); });

app.Run();
