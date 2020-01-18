using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace StupidTodo.Domain
{
    public class Data
    {
        public Data()
        {
            GetTimes = new List<TimeSpan>();
            FirstTimes = new List<TimeSpan>();
            SendOneTimes = new List<TimeSpan>();
            SendTimes = new List<TimeSpan>();
        }


        public List<TimeSpan> GetTimes { get; set; }
        public List<TimeSpan> FirstTimes { get; set; }
        public List<TimeSpan> SendOneTimes { get; set; }
        public List<TimeSpan> SendTimes { get; set; }


        public static void AppendToTarget(Data source, Data target)
        {
            target.FirstTimes.AddRange(source.FirstTimes);
            target.GetTimes.AddRange(source.GetTimes);
            target.SendOneTimes.AddRange(source.SendOneTimes);
            target.SendTimes.AddRange(source.SendTimes);
        }

        public static Data LoadFromFile(FileInfo file)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                return new Data()
                {
                    FirstTimes = ParseLine(reader),
                    GetTimes = ParseLine(reader),
                    SendOneTimes = ParseLine(reader),
                    SendTimes = ParseLine(reader)
                };
            }

            List<TimeSpan> ParseLine(StreamReader reader)
            {
                return reader.ReadLine()
                                .Split(':')
                                .Select(v => TimeSpan.FromMilliseconds(Double.Parse(v)))
                                .ToList();
            }
        }

        public static Data LoadFromFiles(DirectoryInfo sourceDirectory)
        {
            var data = new Data();
            foreach (var file in sourceDirectory.GetFiles($"*{Data.FileExtension}", SearchOption.TopDirectoryOnly))
            {
                AppendToTarget(LoadFromFile(file), data);
            }
            return data;
        }

        public void SaveToFile(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(String.Join(":", GetTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", FirstTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", SendOneTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", SendTimes.Select(t => t.TotalMilliseconds)));
            }
        }


        public const string FileExtension = ".service-compare";
    }
}
