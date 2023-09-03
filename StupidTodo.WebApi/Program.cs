var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<ITodoDataProvider, SimpleTodoDataProvider>()
    .AddSingleton<ITodoApi, TodoApi>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "StupidTodo.WebApi", Version = "v1" }); })
    .AddCors(options => options.AddPolicy("corspolicy", bldr => { bldr.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); }));

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app
    .MapTodoEndpoints()
    .UseCors("corspolicy")
    .UseHttpsRedirection()
    .UseBlazorFrameworkFiles()
    .UseStaticFiles()
    .UseRouting()
    .UseSwagger()
    .UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "StupidTodo.WebApi v1"); })
    .UseEndpoints(endpoints => { endpoints.MapFallbackToFile("index.html"); });

app.Run();
