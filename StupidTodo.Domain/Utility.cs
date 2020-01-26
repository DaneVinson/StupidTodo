using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StupidTodo.Domain
{
    public static class Utility
    {
        public async static Task<Data> TestWebApi(TestingOptions options)
        {
            var stopwatch = new Stopwatch();
            var data = new Data();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Console.WriteLine($"Testing WebApi at {options.WebApiUri} [{options.Iterations} iterations]");

                // Ensure service warm-up
                var warm = await httpClient.GetStringAsync($"{options.WebApiUri}/all");
                var warmResult = await httpClient.PostAsJsonAsync($"{options.WebApiUri}/send", JsonSerializer.Deserialize<Todo[]>(warm));
                warm = await httpClient.GetStringAsync($"{options.WebApiUri}/first");
                warmResult = await httpClient.PostAsJsonAsync($"{options.WebApiUri}/send-one", JsonSerializer.Deserialize<Todo>(warm));

                for (int i = 0; i < options.Iterations; i++)
                {
                    // Get all
                    stopwatch.Start();
                    var json = await httpClient.GetStringAsync($"{options.WebApiUri}/all");
                    var todos = JsonSerializer.Deserialize<Todo[]>(json);
                    stopwatch.Stop();
                    data.GetTimes.Add(stopwatch.Elapsed);

                    // Send all
                    stopwatch.Restart();
                    await httpClient.PostAsJsonAsync($"{options.WebApiUri}/send", todos);
                    stopwatch.Stop();
                    data.SendTimes.Add(stopwatch.Elapsed);

                    // Get first
                    stopwatch.Restart();
                    var todo = JsonSerializer.Deserialize<Todo>(await httpClient.GetStringAsync($"{options.WebApiUri}/first"));
                    stopwatch.Stop();
                    data.FirstTimes.Add(stopwatch.Elapsed);

                    // Send one
                    stopwatch.Restart();
                    await httpClient.PostAsJsonAsync($"{options.WebApiUri}/send-one", todo);
                    stopwatch.Stop();
                    data.SendOneTimes.Add(stopwatch.Elapsed);

                    Console.WriteLine($"WebApi iteration {i + 1} of {options.Iterations} complete");
                }
            }

            return data;
        }
    }
}
