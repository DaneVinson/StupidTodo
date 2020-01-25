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
                //var data = Data.LoadFromFile(new FileInfo(@"C:\Users\dvinson\Documents\GitHub\StupidTodo\service-compare-data\WebApi-100_iterations-run_1.service-compare"));
                //data.SaveStatisticsToFile("c:\\temp\\StupidTodoServiceCompare-WebApi-stats.txt");
                //data = Data.LoadFromFile(new FileInfo(@"C:\Users\dvinson\Documents\GitHub\StupidTodo\service-compare-data\gRPC-100_iterations-run_1.service-compare"));
                //data.SaveStatisticsToFile("c:\\temp\\StupidTodoServiceCompare-gRPC-stats.txt");

                // Test Web API
                if (TestWebApiService)
                {
                    var webApiData = await TestWebApi();
                    webApiData.SaveToFile(GetDataFilePath(webApiData, "WebApi"));
                }

                // Test gRPC
                if (TestGrpcService)
                {
                    var gRpcData = await TestGrpc();
                    gRpcData.SaveToFile(GetDataFilePath(gRpcData, "gRPC"));
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
            using (var channel = GrpcChannel.ForAddress(GrpcUri))
            {
                var client = new TodoSvc.TodoSvcClient(channel);

                Console.WriteLine($"Testing gRPC, {Iterations} iterations");

                // Ensure warm-up
                var warmer = await client.FirstAsync(empty);

                for (int i = 0; i < Iterations; i++)
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

                    // Get then send first Todo multiple times
                    for (int j = 0; j < SingleSendReceiveCount; j++)
                    {
                        stopwatch.Restart();
                        var todo = await client.FirstAsync(empty);
                        stopwatch.Stop();
                        data.FirstTimes.Add(stopwatch.Elapsed);

                        stopwatch.Restart();
                        result = await client.SendOneAsync(todo);
                        stopwatch.Stop();
                        data.SendOneTimes.Add(stopwatch.Elapsed);
                    }

                    Console.WriteLine($"gRPC iteration {i + 1} of {Iterations} complete");
                }
            }

            return data;
        }
        
        private async static Task<Data> TestWebApi()
        {
            var stopwatch = new Stopwatch();
            var data = new Data();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Console.WriteLine($"Testing WebApi, {Iterations} iterations");

                // Ensure service warm-up
                var warm = await httpClient.GetStringAsync($"{WebApiUri}/all");
                var warmResult = await httpClient.PostAsJsonAsync($"{WebApiUri}/send", JsonSerializer.Deserialize<Todo[]>(warm));
                warm = await httpClient.GetStringAsync($"{WebApiUri}/first");
                warmResult = await httpClient.PostAsJsonAsync($"{WebApiUri}/send-one", JsonSerializer.Deserialize<Todo>(warm));

                for (int i = 0; i < Iterations; i++)
                {
                    // Get all
                    stopwatch.Start();
                    var json = await httpClient.GetStringAsync($"{WebApiUri}/all");
                    var todos = JsonSerializer.Deserialize<Todo[]>(json);
                    stopwatch.Stop();
                    data.GetTimes.Add(stopwatch.Elapsed);

                    // Send all
                    stopwatch.Restart();
                    var result = await httpClient.PostAsJsonAsync($"{WebApiUri}/send", todos);
                    stopwatch.Stop();
                    data.SendTimes.Add(stopwatch.Elapsed);
                    if (!result.IsSuccessStatusCode) { Console.WriteLine($"ERROR: Sending all failed. {result.StatusCode}"); }

                    // Get then send first Todo multiple times
                    for (int j = 0; j < SingleSendReceiveCount; j++)
                    {
                        stopwatch.Restart();
                        json = await httpClient.GetStringAsync($"{WebApiUri}/first");
                        var todo = JsonSerializer.Deserialize<Todo>(json);
                        stopwatch.Stop();
                        data.FirstTimes.Add(stopwatch.Elapsed);

                        stopwatch.Restart();
                        result = await httpClient.PostAsJsonAsync($"{WebApiUri}/send-one", todo);
                        stopwatch.Stop();
                        data.SendOneTimes.Add(stopwatch.Elapsed);
                        if (!result.IsSuccessStatusCode) { Console.WriteLine($"ERROR: Sending one failed. {result.StatusCode}"); }
                    }

                    Console.WriteLine($"WebApi iteration {i + 1} of {Iterations} complete");
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
                            DataFilesFolder,
                            $"{prependName}-{data.GetTimes.Count}_iterations-run_{Configuration["Run"]}{Data.FileExtension}");
        }


        static Program()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", false, true)
                                    .Build();
            DataFilesFolder = Configuration["DataFilesFolder"];
            GrpcUri = Configuration["GrpcUri"];
            Iterations = Int32.Parse(Configuration["Iterations"]);
            SingleSendReceiveCount = Int32.Parse(Configuration["SingleSendReceiveCount"]);
            var testTypes = Configuration["TestType"].Split(',');
            TestGrpcService = testTypes.FirstOrDefault(s => s == "grpc") != null;
            TestWebApiService = testTypes.FirstOrDefault(s => s == "webapi") != null;
            WebApiUri = Configuration["WebApiUri"];
        }

        private static readonly IConfiguration Configuration;
        private static readonly string DataFilesFolder;
        private static readonly string GrpcUri;
        private static readonly int Iterations;
        private static readonly int SingleSendReceiveCount;
        private static readonly bool TestGrpcService;
        private static readonly bool TestWebApiService;
        private static readonly string WebApiUri;
    }
}
