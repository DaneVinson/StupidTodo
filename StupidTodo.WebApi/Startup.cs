using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StupidTodo.Domain;

namespace StupidTodo.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) =>
            _configuration = configuration;

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app
                    .UseDeveloperExceptionPage()
                    .UseSwagger()
                    .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StupidTodo.WebApi v1"));
            }

            app
                .UseHttpsRedirection()
                .UseBlazorFrameworkFiles()
                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapFallbackToFile("index.html");
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ITodoDataProvider>(new SimpleTodoDataProvider())
                .AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "StupidTodo.WebApi", Version = "v1" }); })
                .AddControllers();
        }
    }
}
