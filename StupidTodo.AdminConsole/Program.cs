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


        private static async Task<TimeSpan> RunTestIteration<TArg, TResult>(
            int iteration, 
            TArg arg, 
            Func<TArg, Task<TResult>> testMethod)
        {
            Watch.Restart();
            _ = await testMethod.Invoke(arg);
            Watch.Stop();
            return Watch.Elapsed;
        }

        private static async Task<Data> TestGrpc()
        {
            Console.Write($"Testing gRPC, {Options.Iterations} iterations");

            var data = new Data();
            var empty = new EmptyMessage();
            using (var channel = GrpcChannel.ForAddress(Options.GrpcUri))
            {
                var client = new TodoSvc.TodoSvcClient(channel);

                // Ensure warm-up
                var warmupTodos = await client.GetAsync(empty);
                Console.WriteLine($", {warmupTodos.Todos.Count} Todo objects");
                using (var stream = client.GetStreaming(empty))
                {
                    await foreach (var message in stream.ResponseStream.ReadAllAsync())
                    {
                    }
                }
                var result = await client.SendAsync(warmupTodos);
                using (var call = client.SendStreaming())
                {
                    foreach (var todo in warmupTodos.Todos)
                    {
                        await call.RequestStream.WriteAsync(todo);
                    }
                    await call.RequestStream.CompleteAsync();
                }
                var warmupTodo = await client.FirstAsync(empty);
                result = await client.SendOneAsync(warmupTodo);

                for (int i = 0; i < Options.Iterations; i++)
                {
                    data.GetTimes.Add(await RunTestIteration(i, empty, async m => await client.GetAsync(m)));
                    data.SendTimes.Add(await RunTestIteration(i, warmupTodos, async m => await client.SendAsync(m)));
                    data.FirstTimes.Add(await RunTestIteration(i, empty, async m => await client.FirstAsync(m)));
                    data.SendOneTimes.Add(await RunTestIteration(i, warmupTodo, async m => await client.SendOneAsync(m)));
                    data.GetStreamingTimes.Add(await RunTestIteration(i, empty, async m =>
                    {
                        await client.GetAsync(m);
                        var todosMessage = new TodosMessage();
                        using (var stream = client.GetStreaming(empty))
                        {
                            await foreach (var message in stream.ResponseStream.ReadAllAsync())
                            {
                                todosMessage.Todos.Add(message);
                            }
                        }
                        return todosMessage;
                    }));
                    data.SendStreamingTimes.Add(await RunTestIteration(i, warmupTodos, async m =>
                    {
                        using (var call = client.SendStreaming())
                        {
                            foreach (var todo in m.Todos)
                            {
                                await call.RequestStream.WriteAsync(todo);
                            }
                            await call.RequestStream.CompleteAsync();
                        }
                        return new ResultMessage() { Success = true };
                    }));
                    WriteIterationLine(i + 1);
                }
            }

            return data;
        }

        private static async Task<Data> TestWcf()
        {
            Console.Write($"Testing WCF, {Options.Iterations} iterations");

            var data = new Data();
            TodoServiceClient client = new TodoServiceClient();
            try
            {
                // Ensure warm-up
                var warmupTodos = await client.GetAsync();
                Console.WriteLine($", {warmupTodos.Length} Todo objects");
                var result = await client.SendAsync(warmupTodos);
                var warmupTodo = await client.FirstAsync();
                result = await client.SendOneAsync(warmupTodo);

                for (int i = 0; i < Options.Iterations; i++)
                {
                    data.GetTimes.Add(await RunTestIteration(i, string.Empty, _ => client.GetAsync()));
                    data.SendTimes.Add(await RunTestIteration(i, warmupTodos, t => client.SendAsync(t)));
                    data.FirstTimes.Add(await RunTestIteration(i, string.Empty, _ => client.FirstAsync()));
                    data.SendOneTimes.Add(await RunTestIteration(i, warmupTodo, t => client.SendOneAsync(t)));
                    WriteIterationLine(i + 1);
                }
            }
            finally { client?.Close(); }

            return data;
        }

        public static async Task<Data> TestWebApi(bool apiIsFramework = false)
        {
            var uri = apiIsFramework ? Options.WebApiFrameworkUri : Options.WebApiUri;
            var framework = apiIsFramework ? ".NET Framework" : ".NET Core";
            Console.Write($"Testing {framework} Web API at {uri} [{Options.Iterations} iterations]");

            var data = new Data();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Ensure service warm-up
                var warmupJson = await httpClient.GetStringAsync($"{uri}/all");
                var warmupTodos = JsonSerializer.Deserialize<Todo[]>(warmupJson);
                Console.WriteLine($", {warmupTodos.Length} Todo objects");
                _ = await httpClient.PostAsJsonAsync($"{uri}/send", warmupTodos);
                warmupJson = await httpClient.GetStringAsync($"{uri}/first");
                var warmupTodo = JsonSerializer.Deserialize<Todo>(warmupJson);
                _ = await httpClient.PostAsJsonAsync($"{uri}/send-one", JsonSerializer.Deserialize<Todo>(warmupJson));

                for (int i = 0; i < Options.Iterations; i++)
                {
                    data.GetTimes.Add(await RunTestIteration(i, string.Empty, async _ =>
                    {
                        var json = await httpClient.GetStringAsync($"{uri}/all");
                        return JsonSerializer.Deserialize<Todo[]>(json);
                    }));
                    data.SendTimes.Add(await RunTestIteration(i, warmupTodos, async t =>
                    {
                        await httpClient.PostAsJsonAsync($"{uri}/send", t);
                        return true;
                    }));
                    data.FirstTimes.Add(await RunTestIteration(i, string.Empty, async _ =>
                    {
                        var json = await httpClient.GetStringAsync($"{uri}/first");
                        return JsonSerializer.Deserialize<Todo>(json);
                    }));
                    data.SendOneTimes.Add(await RunTestIteration(i, warmupTodo, async t =>
                    {
                        await httpClient.PostAsJsonAsync($"{uri}/send-one", t);
                        return true;
                    }));
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
            Watch = new Stopwatch();
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
        private static readonly Stopwatch Watch;
    }
}
