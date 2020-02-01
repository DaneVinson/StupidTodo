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
using System.ServiceModel;
using System.Text.Json;
using System.Threading.Tasks;
using TodoServiceReference;

namespace StupidTodo.AdminConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Data data = null;

            try
            {
                //WriteGenFuData(5000);

                foreach (var test in TestTypes)
                {
                    var start = DateTime.Now;
                    data = test switch
                    {
                        "grpc" => await TestGrpc(),
                        "webapiframework" => await TestWebApi(true),
                        "webapi" => await TestWebApi(),
                        "wcf" => await TestWcf(),
                        _ => throw new ArgumentException($"'{test}' is and unknown test type")
                    };
                    Console.WriteLine($"{test} test completion in {DateTime.Now - start}");
                    data.SaveToFile(GetDataFilePath(test, data.GetTimes.Count));
                    data.SaveStatisticsToFile(GetStatisticsFilePath(test));
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


        private async static Task<Data> TestGrpc()
        {
            var stopwatch = new Stopwatch();
            var data = new Data();
            var empty = new EmptyMessage();
            using (var channel = GrpcChannel.ForAddress(Options.GrpcUri))
            {
                var client = new TodoSvc.TodoSvcClient(channel);

                Console.Write($"Testing gRPC, {Options.Iterations} iterations");

                // Ensure warm-up
                var warmup = await client.GetAsync(empty);
                Console.WriteLine($", {warmup.Todos.Count} Todo objects");
                using (var stream = client.GetStreaming(empty))
                {
                    await foreach (var message in stream.ResponseStream.ReadAllAsync())
                    {
                    }
                }
                var result = await client.SendAsync(warmup);
                using (var call = client.SendStreaming())
                {
                    foreach (var todo in warmup.Todos)
                    {
                        await call.RequestStream.WriteAsync(todo);
                    }
                    await call.RequestStream.CompleteAsync();
                }
                var warm = await client.FirstAsync(empty);
                result = await client.SendOneAsync(warm);

                for (int i = 0; i < Options.Iterations; i++)
                {
                    var todos = new List<TodoMessage>();

                    // Get all streaming
                    stopwatch.Start();
                    using (var stream = client.GetStreaming(empty))
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
                    var todosMessage = await client.GetAsync(empty);
                    todos.AddRange(todosMessage.Todos);
                    stopwatch.Stop();
                    data.GetTimes.Add(stopwatch.Elapsed);

                    // Send all streaming
                    stopwatch.Restart();
                    using (var call = client.SendStreaming())
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
                    result = await client.SendAsync(todosMessage);
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

                    WriteIterationLine(i + 1);
                }
            }

            return data;
        }

        private static async Task<Data> TestWcf()
        {
            var stopwatch = new Stopwatch();
            var data = new Data();

            Console.Write($"Testing WCF, {Options.Iterations} iterations");

            TodoServiceClient client = new TodoServiceClient();
            try
            {
                // Ensure warm-up
                var warmup = await client.GetAsync();
                Console.WriteLine($", {warmup.Length} Todo objects");
                var result = await client.SendAsync(warmup);
                var warm = await client.FirstAsync();
                result = await client.SendOneAsync(warm);

                for (int i = 0; i < Options.Iterations; i++)
                {
                    // Get all
                    stopwatch.Start();
                    var todos = await client.GetAsync();
                    stopwatch.Stop();
                    data.GetTimes.Add(stopwatch.Elapsed);

                    // Send all
                    stopwatch.Restart();
                    _ = await client.SendAsync(todos);
                    stopwatch.Stop();
                    data.SendTimes.Add(stopwatch.Elapsed);

                    // Get first
                    stopwatch.Restart();
                    var todo = await client.FirstAsync();
                    stopwatch.Stop();
                    data.FirstTimes.Add(stopwatch.Elapsed);

                    // Send one
                    stopwatch.Restart();
                    _ = await client.SendOneAsync(todo);
                    stopwatch.Stop();
                    data.SendOneTimes.Add(stopwatch.Elapsed);

                    WriteIterationLine(i + 1);
                }
            }
            finally { client?.Close(); }
           
            return data;
        }

        public static async Task<Data> TestWebApi(bool apiIsFramework = false)
        {
            var stopwatch = new Stopwatch();
            var data = new Data();
            var uri = apiIsFramework ? Options.WebApiFrameworkUri : Options.WebApiUri;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var framework = apiIsFramework ? ".NET Framework" : ".NET Core";
                Console.Write($"Testing {framework} Web API at {uri} [{Options.Iterations} iterations]");

                // Ensure service warm-up
                var warmJson = await httpClient.GetStringAsync($"{uri}/all");
                var warmupTodos = JsonSerializer.Deserialize<Todo[]>(warmJson);
                Console.WriteLine($", {warmupTodos.Length} Todo objects");
                var warmResult = await httpClient.PostAsJsonAsync($"{uri}/send", warmupTodos);
                warmJson = await httpClient.GetStringAsync($"{uri}/first");
                warmResult = await httpClient.PostAsJsonAsync($"{uri}/send-one", JsonSerializer.Deserialize<Todo>(warmJson));

                for (int i = 0; i < Options.Iterations; i++)
                {
                    // Get all
                    stopwatch.Start();
                    var json = await httpClient.GetStringAsync($"{uri}/all");
                    var todos = JsonSerializer.Deserialize<Todo[]>(json);
                    stopwatch.Stop();
                    data.GetTimes.Add(stopwatch.Elapsed);

                    // Send all
                    stopwatch.Restart();
                    await httpClient.PostAsJsonAsync($"{uri}/send", todos);
                    stopwatch.Stop();
                    data.SendTimes.Add(stopwatch.Elapsed);

                    // Get first
                    stopwatch.Restart();
                    var todo = JsonSerializer.Deserialize<Todo>(await httpClient.GetStringAsync($"{uri}/first"));
                    stopwatch.Stop();
                    data.FirstTimes.Add(stopwatch.Elapsed);

                    // Send one
                    stopwatch.Restart();
                    await httpClient.PostAsJsonAsync($"{uri}/send-one", todo);
                    stopwatch.Stop();
                    data.SendOneTimes.Add(stopwatch.Elapsed);

                    WriteIterationLine(i + 1);
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

        private static string GetDataFilePath(string testName, int iterations)
        {
            return Path.Combine(
                            Options.DataFilesFolder,
                            $"{testName}-{iterations}_iterations{Data.FileExtension}");
        }

        private static string GetStatisticsFilePath(string testName)
        {
            return Path.Combine(
                            Options.DataFilesFolder,
                            $"StupidTodoServiceCompare-{testName}-stats.txt");
        }

        private static void WriteIterationLine(int iteration)
        {
            Console.WriteLine($"{iteration} / {Options.Iterations}");
        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", false, true)
                                    .Build();
            TestTypes = Configuration["TestTypes"].Split(',');
            Options = new TestingOptions()
            {
                DataFilesFolder = Configuration["DataFilesFolder"],
                GrpcUri = Configuration["GrpcUri"],
                Iterations = Int32.Parse(Configuration["Iterations"]),
                WebApiFrameworkUri = Configuration["WebApiFrameworkUri"],
                WebApiUri = Configuration["WebApiUri"]
            };
        }


        private static readonly IConfiguration Configuration;
        private static readonly TestingOptions Options;
        private static readonly string[] TestTypes;
    }
}
