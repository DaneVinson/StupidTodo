using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StupidTodo.Client.Blazor.Core.ViewModels;
using StupidTodo.Domain;
using StupidTodo.Service.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.Server.Blazor
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services
                .AddSingleton<ITodoDataProvider>(new SimpleTodoDataProvider())
                .AddSingleton<ITodoApi, TodoController>()
                .AddSingleton<TodosViewModel>()
                .AddTransient<TodoViewModel>()
                .AddTransient<DoneViewModel>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
        }
    }
}
