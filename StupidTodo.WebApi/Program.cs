using StupidTodo.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // .AddSwaggerGen(options =>
    // {
    //     options.SwaggerDoc("v1", new OpenApiInfo {Title = "StupidTodo.WebApi", Version = "v1"});
    // })
    .AddSingleton<ITodoDataProvider>(new SimpleTodoDataProvider());

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app
    .MapTodoEndpoints()
    .UseHttpsRedirection()
    // .UseSwagger()
    // .UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "StupidTodo.WebApi v1"); })
    .UseStaticFiles();

app.Run();
