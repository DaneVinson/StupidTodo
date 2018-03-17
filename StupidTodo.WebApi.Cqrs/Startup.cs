using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StupidTodo.Data.AzureTableStorage;
using StupidTodo.Domain;
using StupidTodo.Domain.EventSource;

namespace StupidTodo.WebApi.Cqrs
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

            app.UseMvc()
                .UseCors(builder => builder.WithOrigins("*")
                                            .AllowAnyMethod()
                                            .AllowAnyHeader()
                                            .AllowCredentials());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var azureTableStorageOptions = new AzureTableStorageOptions();
            Configuration.GetSection("AzureTableStorage").Bind(azureTableStorageOptions);

            services.AddSingleton(azureTableStorageOptions)
                    .AddTransient<IProjector<Todo>, AzureTableStorageTodoProjector>()
                    .AddTransient<IEventStore, AzureTableStorageEventStore>()
                    .AddTransient<IDispatcher<ICommand>, CommandDispatcher<Todo>>()
                    .AddTransient<IQueueWriter<ICommand>, CommandQueueWriter>()
                    .AddCors()
                    .AddMvc();
        }


        public IConfiguration Configuration { get; }
    }
}
