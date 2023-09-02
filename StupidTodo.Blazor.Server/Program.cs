var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services
	.AddSingleton<ITodoDataProvider, SimpleTodoDataProvider>()
	.AddSingleton<ITodoApi, TodoApi>()
	.AddSingleton<TodosViewModel>()
	.AddTransient<TodoViewModel>()
	.AddTransient<DoneViewModel>();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app
		.UseExceptionHandler("/Error")
		.UseHsts();
}

app
	.UseHttpsRedirection()
	.UseStaticFiles()
	.UseRouting()
	.UseAuthorization()
	.UseEndpoints(endpoints =>
	{
		endpoints.MapBlazorHub();
		endpoints.MapFallbackToPage("/BlazorHost");
	});

app.Run();
