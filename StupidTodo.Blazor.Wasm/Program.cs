using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using StupidTodo.Blazor.Core.ViewModels;
using StupidTodo.Domain;
using StupidTodo.Blazor.Core;

namespace StupidTodo.Blazor.Wasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services
                    .AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                    .AddSingleton<ITodoApi, TodoHttpApi>()
                    .AddSingleton<TodosViewModel>()
                    .AddTransient<TodoViewModel>()
                    .AddTransient<DoneViewModel>();

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
