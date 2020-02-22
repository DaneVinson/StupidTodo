using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using ProtoBuf.Grpc.Client;
using StupidTodo.Domain;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.ProtobutNet.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {                
                using (var channel = GrpcChannel.ForAddress(Configuration["ServiceUri"]))
                {
                    var todoService = channel.CreateGrpcService<ITodoService>();

                    var todos = await todoService.GetTodos();
                    todos.ToList().ForEach(t => Console.WriteLine(t));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} - {1}", ex.GetType(), ex.Message);
                Console.WriteLine(ex.StackTrace ?? String.Empty);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("...");
                Console.ReadKey();
            }
        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", false, true)
                                    .Build();
        }

        private static readonly IConfiguration Configuration;
    }
}
