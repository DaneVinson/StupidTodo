using GenFu;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using StupidTodo.Domain;
using StupidTodo.Grpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace StupidTodo.AdminConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Test Web API
                if (TestWebApiService)
                {
                    var webApiData = await Utility.TestWebApi(Options);
                    webApiData.SaveToFile(GetDataFilePath(webApiData, "WebApi"));
                    webApiData.SaveStatisticsToFile("c:\\temp\\StupidTodoServiceCompare-WebApi-stats.txt");
                }

                // Test gRPC
                if (TestGrpcService)
                {
                    var gRpcData = await TestGrpc();
                    gRpcData.SaveToFile(GetDataFilePath(gRpcData, "gRPC"));
                    gRpcData.SaveStatisticsToFile("c:\\temp\\StupidTodoServiceCompare-gRPC-stats.txt");
                }

                //WriteGenFuData(5000);
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


        private async static Task<Data> TestGrpc()
        {
            var stopwatch = new Stopwatch();
            var data = new Data();
            var empty = new EmptyMessage();
            using (var channel = GrpcChannel.ForAddress(Options.GrpcUri))
            {
                var client = new TodoSvc.TodoSvcClient(channel);

                Console.WriteLine($"Testing gRPC, {Options.Iterations} iterations");

                // Ensure warm-up
                var warmer = await client.FirstAsync(empty);

                for (int i = 0; i < Options.Iterations; i++)
                {
                    var todos = new List<TodoMessage>();

                    // Get all streaming
                    stopwatch.Start();
                    using (var stream = client.Get(empty))
                    {
                        await foreach (var message in stream.ResponseStream.ReadAllAsync())
                        {
                            todos.Add(message);
                        }
                    }
                    stopwatch.Stop();
                    data.GetStreamingTimes.Add(stopwatch.Elapsed);
                    todos.Clear();

                    // Get all in one package
                    stopwatch.Start();
                    var todosMessage = await client.GetPackageAsync(empty);
                    todos.AddRange(todosMessage.Todos);
                    stopwatch.Stop();
                    data.GetTimes.Add(stopwatch.Elapsed);

                    // Send all streaming
                    stopwatch.Restart();
                    using (var call = client.Send())
                    {
                        foreach (var todo in todos)
                        {
                            await call.RequestStream.WriteAsync(todo);
                        }
                        await call.RequestStream.CompleteAsync();
                    }
                    stopwatch.Stop();
                    data.SendStreamingTimes.Add(stopwatch.Elapsed);

                    // Send all in one package
                    stopwatch.Start();
                    var result = await client.SendPackageAsync(todosMessage);
                    stopwatch.Stop();
                    data.SendTimes.Add(stopwatch.Elapsed);

                    // Get first
                    stopwatch.Restart();
                    var firstTodo = await client.FirstAsync(empty);
                    stopwatch.Stop();
                    data.FirstTimes.Add(stopwatch.Elapsed);

                    // Send one
                    stopwatch.Restart();
                    result = await client.SendOneAsync(firstTodo);
                    stopwatch.Stop();
                    data.SendOneTimes.Add(stopwatch.Elapsed);

                    Console.WriteLine($"gRPC iteration {i + 1} of {Options.Iterations} complete");
                }
            }

            return data;
        }
        

        private static List<Todo> GenFuTodos(int count)
        {
            GenFu.GenFu
                .Configure<Todo>()
                .Fill(t => t.Id, () => Guid.NewGuid().ToString())
                .Fill(t => t.UserId, () => Bilbo.Id);

            return A.ListOf<Todo>(count);
        }

        private static void WriteGenFuData(int count)
        {
            var todos = GenFuTodos(count);
            var jsonTodos = JsonSerializer.Serialize(todos.ToArray());
            var jsonFromStorage = String.Empty;
            string path = "c:\\temp\\genfu-data.json";
            using (var writer = new StreamWriter(path))
            {
                writer.Write(jsonTodos);
            }
            Console.WriteLine($"GenFu data written to {path}");
        }

        private static string GetDataFilePath(Data data, string prependName)
        {
            return Path.Combine(
                            Options.DataFilesFolder,
                            $"{prependName}-{data.GetTimes.Count}_iterations{Data.FileExtension}");
        }


        static Program()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", false, true)
                                    .Build();
            var testTypes = Configuration["TestTypes"].Split(',');
            TestGrpcService = testTypes.FirstOrDefault(s => s == "grpc") != null;
            TestWebApiService = testTypes.FirstOrDefault(s => s == "webapi") != null;
            Options = new TestingOptions()
            {
                DataFilesFolder = Configuration["DataFilesFolder"],
                GrpcUri = Configuration["GrpcUri"],
                Iterations = Int32.Parse(Configuration["Iterations"]),
                WebApiUri = Configuration["WebApiUri"]
            };
        }

        private static readonly IConfiguration Configuration;
        private static readonly TestingOptions Options;
        private static readonly bool TestGrpcService;
        private static readonly bool TestWebApiService;
    }
}
