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

            app.UseMvc()
                .UseCors(builder => builder.WithOrigins("*")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ExploreConfiguration(services)
                .AddCors()
                .AddMvc();
        }

        private IServiceCollection ExploreConfiguration(IServiceCollection services)
        {
            // Get a simple setting, 
            //  case insensitive
            //  recommend standardizing naming convention
            IConfigurationSection section = Configuration.GetSection("SIMPLEsetting");

            // multi level
            string setting1 = Configuration.GetSection("SIMPLEGraph")["simpleGRAPHsetting1"];

            // better way
            IConfigurationSection s2 = Configuration.GetSection("simplegraph:simplegraphsetting1");

            // even better way
            string s3 = Configuration["simplegraph:simplegraphsetting1"];

            string s4 = Configuration["configName"];
            


            // Bind to and object e.g.
            // var foo = new Foo();
            // Config.Bind(foo, "sectionName");

            // From book

            return services;
        }

        public IConfiguration Configuration { get; }
    }
}
