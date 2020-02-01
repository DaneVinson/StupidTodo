using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StupidTodo.Domain;
using StupidTodo.GrpcService;

namespace StupidTodo.Grpc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<TodoService>();

                    // custom routing?
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            GenFuTodoDataProvider.LoadDataFile(Configuration["DataFilePath"]);

            services.AddSingleton<IServiceCompareDataProvider>(new GenFuTodoDataProvider())
                    .AddSingleton(new GrpcDataProvider())
                    .AddGrpc();
        }

        private IConfiguration Configuration { get; }
    }
}
