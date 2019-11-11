using Grpc.Core;
using Grpc.Net.Client;
using StupidTodo.GrpcService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StupidTodo.GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
                {
                    var client = new TodoSvc.TodoSvcClient(channel);

                    // GetTodos
                    Console.WriteLine("Calling GetTodos");
                    var todoMessages = new List<TodoMessage>();
                    using (var stream = client.GetTodos(new Empty()))
                    {
                        await foreach(var message in stream.ResponseStream.ReadAllAsync())
                        {
                            Console.WriteLine($"    {Utility.TodoFromTodoMessage(message)}");
                            todoMessages.Add(message);
                        }
                    }
                    Console.WriteLine();

                    // GetDoneTodos
                    Console.WriteLine("Calling GetDoneTodos");
                    using (var stream = client.GetDoneTodos(new Empty()))
                    {
                        await foreach (var message in stream.ResponseStream.ReadAllAsync())
                        {
                            Console.WriteLine($"    {Utility.TodoFromTodoMessage(message)}");
                            todoMessages.Add(message);
                        }
                    }
                    Console.WriteLine();

                    // UpdateTodo
                    var firstTodo = todoMessages.First();
                    Console.WriteLine($"Calling UpdateTodo {Utility.TodoFromTodoMessage(firstTodo)}");
                    firstTodo.Done = true;
                    firstTodo = await client.UpdateTodoAsync(firstTodo);
                    Console.WriteLine($"Result: {Utility.TodoFromTodoMessage(firstTodo)}");
                    Console.WriteLine();

                    // AddTodo
                    var newTodo = new TodoMessage()
                    {
                        Description = "Get this done!",
                        Done = false,
                        Id = "23",
                        UserId = Bilbo.Id
                    };
                    Console.WriteLine($"Calling AddTodo {Utility.TodoFromTodoMessage(newTodo)}");
                    newTodo = await client.AddTodoAsync(newTodo);
                    Console.WriteLine($"Result: {Utility.TodoFromTodoMessage(newTodo)}");
                    Console.WriteLine();

                    // DeleteTodo
                    Console.WriteLine($"Calling DeleteTodo for Id {newTodo.Id}");
                    var success = await client.DeleteTodoAsync(new IdMessage() { Value = newTodo.Id });
                    Console.WriteLine($"Result: {success}");
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
    }
}
