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
            GetStreamingTimes = new List<TimeSpan>();
            FirstTimes = new List<TimeSpan>();
            SendOneTimes = new List<TimeSpan>();
            SendStreamingTimes = new List<TimeSpan>();
            SendTimes = new List<TimeSpan>();
        }


        public List<TimeSpan> GetTimes { get; set; }
        public List<TimeSpan> GetStreamingTimes { get; set; }
        public List<TimeSpan> FirstTimes { get; set; }
        public List<TimeSpan> SendOneTimes { get; set; }
        public List<TimeSpan> SendStreamingTimes { get; set; }
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
                    GetTimes = ReadAndParseLine(reader),
                    GetStreamingTimes = ReadAndParseLine(reader),
                    SendTimes = ReadAndParseLine(reader),
                    SendStreamingTimes = ReadAndParseLine(reader),
                    FirstTimes = ReadAndParseLine(reader),
                    SendOneTimes = ReadAndParseLine(reader)
                };
            }

            List<TimeSpan> ReadAndParseLine(StreamReader reader)
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

        public void SaveStatisticsToFile(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                WriteLine(writer, $"{nameof(GetTimes)}", GetTimes.Select(t => t.TotalMilliseconds));
                WriteLine(writer, $"{nameof(GetStreamingTimes)}", GetStreamingTimes.Select(t => t.TotalMilliseconds));
                WriteLine(writer, $"{nameof(SendTimes)}", SendTimes.Select(t => t.TotalMilliseconds));
                WriteLine(writer, $"{nameof(SendStreamingTimes)}", SendStreamingTimes.Select(t => t.TotalMilliseconds));
                WriteLine(writer, $"{nameof(FirstTimes)}", FirstTimes.Select(t => t.TotalMilliseconds));
                WriteLine(writer, $"{nameof(SendOneTimes)}", SendOneTimes.Select(t => t.TotalMilliseconds));
            }

            void WriteLine(StreamWriter writer, string propertyName, IEnumerable<double> times)
            {
                writer.WriteLine(String.Format(
                                            "[{0}] Count:{1}, Average:{2}, StandardDeviation:{3}, Min:{4}, Max:{5}",
                                            propertyName,
                                            times.Count(),
                                            times.Average(),
                                            GetStandardDeviation(times),
                                            times.Min(),
                                            times.Max()));
            }
        }

        public void SaveToFile(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(String.Join(":", GetTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", GetStreamingTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", SendTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", SendStreamingTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", FirstTimes.Select(t => t.TotalMilliseconds)));
                writer.WriteLine(String.Join(":", SendOneTimes.Select(t => t.TotalMilliseconds)));
            }
        }

        private static double GetStandardDeviation(IEnumerable<double> values)
        {
            var mean = values.Average();
            return Math.Sqrt(values.Select(v => Math.Pow((v - mean), 2)).Average());
        }


        public const string FileExtension = ".service-compare";
    }
}
