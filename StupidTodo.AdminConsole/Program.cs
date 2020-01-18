using GenFu;
using Microsoft.Extensions.Configuration;
using StupidTodo.Domain;
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
                // Increment each run for new data files
                var run = 1;

                // Test Web API
                var data = await TestWebApi();
                data.SaveToFile(Path.Combine(DataFilesFolder, $"WebApi-{data.GetTimes.Count}_iterations-run_{run}{Data.FileExtension}"));

                // Test gRPC
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

                // Ensure service warm-up
                var warmer = await httpClient.GetStringAsync($"{WebApiUri}/first");

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


        static Program()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", false, true)
                                    .Build();
            DataFilesFolder = Configuration["DataFilesFolder"];
            Iterations = Int32.Parse(Configuration["Iterations"]);
            SingleSendReceiveCount = Int32.Parse(Configuration["SingleSendReceiveCount"]);
            WebApiUri = Configuration["WebApiUri"];
        }

        private static readonly IConfiguration Configuration;
        private static readonly string DataFilesFolder;
        private static readonly int Iterations;
        private static readonly int SingleSendReceiveCount;
        private static readonly string WebApiUri;
    }
}
