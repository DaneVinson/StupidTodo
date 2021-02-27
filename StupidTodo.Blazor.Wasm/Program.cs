using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using StupidTodo.Domain;
using Fluxor;
using StupidTodo.Blazor.Core.Store.States;
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
                    .AddFluxor(options => options.ScanAssemblies(typeof(Program).Assembly, typeof(Main).Assembly));

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
