namespace StupidTodo.Blazor.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp
						.CreateBuilder()
						.UseMauiApp<App>()
						.ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Configuration.AddConfiguration(new ConfigurationBuilder()
													.AddJsonFile("appsettings.json", optional: false)
													.Build());

		builder.Services
			.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.Configuration["WebApiBaseUri"] ?? string.Empty) })
			.AddSingleton<ITodoApi, HttpTodoApi>()
			.AddSingleton<ITodoDataProvider, SimpleTodoDataProvider>()
			.AddSingleton<TodosViewModel>()
			.AddTransient<TodoViewModel>()
			.AddTransient<DoneViewModel>();

		return builder.Build();
	}
}
