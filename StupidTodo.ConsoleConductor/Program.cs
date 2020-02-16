using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StupidTodo.ConsoleConductor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                for (int i = 0; i < ClientCount; i++)
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        Arguments = (i + 1).ToString(),
                        WindowStyle = ProcessWindowStyle.Normal,
                        FileName = ClientExeFile.FullName,
                        UseShellExecute = true,
                        WorkingDirectory = ClientExeFile.DirectoryName
                    };
                    var process = Process.Start(startInfo);
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
            int.TryParse(Configuration["ClientCount"], out ClientCount);
            ClientExeFile = new FileInfo(Configuration["ClientExeFile"]);
        }

        private static readonly int ClientCount;
        private static readonly FileInfo ClientExeFile;
        private static readonly IConfiguration Configuration;
    }
}
