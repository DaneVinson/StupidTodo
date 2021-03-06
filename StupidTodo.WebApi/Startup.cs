using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StupidTodo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection()
                .UseSwagger()
                .UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "StupidTodo.WebApi v1"); })
                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ITodoDataProvider>(new SimpleTodoDataProvider())
                .AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "StupidTodo.WebApi", Version = "v1" }); })
                .AddControllers();
        }

        private IConfiguration Configuration { get; }
    }
}
