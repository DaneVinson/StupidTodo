var builder = WebAssemblyHostBuilder.CreateDefault(args);

var webApiBaseUri = builder.Configuration["WebApiBaseUri"] ?? builder.HostEnvironment.BaseAddress;

builder.Services
		.AddSingleton(new HttpClient { BaseAddress = new Uri(webApiBaseUri) })
        .AddSingleton<ITodoApi, HttpTodoApi>()
        .AddSingleton<TodosViewModel>()
        .AddTransient<TodoViewModel>()
        .AddTransient<DoneViewModel>();

builder.RootComponents.Add<App>("#app");

await builder.Build().RunAsync();
