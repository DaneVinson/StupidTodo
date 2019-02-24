using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using StupidTodo.AzureStorage;
using StupidTodo.Domain;

namespace StupidTodo.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles()
                .UseMvc()
                .UseCors(builder => builder.WithOrigins("*")
                                            .AllowAnyMethod()
                                            .AllowAnyHeader()
                                            .AllowCredentials());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration.GetSection("AzureStorage")?.Get<AzureStorageOptions>())
                    .AddSingleton(Configuration.GetSection("User")?.Get<UserOptions>())
                    .AddSingleton(new ClientBuilder().UseLocalhostClustering()
                                                        .Configure<ClusterOptions>(options =>
                                                        {
                                                            options.ClusterId = "development";
                                                            options.ServiceId = "StupidTodo";
                                                        })
                                                        .ConfigureLogging(logging => logging.AddConsole())
                                                        .Build())
                    .AddTransient<ITodoRepository, TodoRepository>();

            services.AddCors()
                    .AddMvc();

            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "development";
                    options.ServiceId = "StupidTodo";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        }


        public IConfiguration Configuration { get; }
    }
}
