using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using StupidTodo.Domain;

namespace StupidTodo.ProtobufNet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpsRedirection()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<TodoService>();
                });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITodoDataProvider, SimpleTodoDataProvider>();

            services.AddCodeFirstGrpc(options =>
            {
                options.ResponseCompressionLevel = CompressionLevel.Optimal;
            });
        }


        private readonly IConfiguration Configuration;
    }
}
