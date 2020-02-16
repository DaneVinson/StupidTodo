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
            target.GetStreamingTimes.AddRange(source.GetStreamingTimes);
            target.GetTimes.AddRange(source.GetTimes);
            target.SendOneTimes.AddRange(source.SendOneTimes);
            target.SendStreamingTimes.AddRange(source.SendStreamingTimes);
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
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) { return new List<TimeSpan>(); }

                return line.Split(':')
                            .Select(v => TimeSpan.FromMilliseconds(double.Parse(v)))
                            .ToList();
            }
        }

        public static Data LoadFromFiles(IEnumerable<FileInfo> files)
        {
            var data = new Data();
            foreach (var file in files)
            {
                AppendToTarget(LoadFromFile(file), data);
            }
            return data;
        }

        public static Data LoadFromFiles(DirectoryInfo sourceDirectory)
        {
            return LoadFromFiles(sourceDirectory.GetFiles($"*{Data.FileExtension}", SearchOption.TopDirectoryOnly));
        }

        public void SaveStatisticsToFile(string path)
        {
            using (var writer = new StreamWriter(path, true))
            {
                writer.WriteLine($"{DateTime.Now}, {GetTimes.Count} iterations");
                WriteTimingLine(writer, $"{nameof(GetTimes)}", GetTimes.Select(t => t.TotalMilliseconds));
                WriteTimingLine(writer, $"{nameof(GetStreamingTimes)}", GetStreamingTimes.Select(t => t.TotalMilliseconds));
                WriteTimingLine(writer, $"{nameof(SendTimes)}", SendTimes.Select(t => t.TotalMilliseconds));
                WriteTimingLine(writer, $"{nameof(SendStreamingTimes)}", SendStreamingTimes.Select(t => t.TotalMilliseconds));
                WriteTimingLine(writer, $"{nameof(FirstTimes)}", FirstTimes.Select(t => t.TotalMilliseconds));
                WriteTimingLine(writer, $"{nameof(SendOneTimes)}", SendOneTimes.Select(t => t.TotalMilliseconds));
                writer.WriteLine();
            }

            void WriteTimingLine(StreamWriter writer, string propertyName, IEnumerable<double> times)
            {
                if (times.Any())
                {
                    writer.WriteLine(String.Format(
                                                "{0} Avg:{1}, StdDev:{2}, Min:{3}, Max:{4}",
                                                propertyName.PadRight(20, ' '),                                                
                                                times.Average(),
                                                GetStandardDeviation(times),
                                                times.Min(),
                                                times.Max()));
                }
                else { writer.WriteLine($"{propertyName.PadRight(20, ' ')} N/A"); }
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

        public static double GetStandardDeviation(IEnumerable<double> values)
        {
            var mean = values.Average();
            return Math.Sqrt(values.Select(v => Math.Pow((v - mean), 2)).Average());
        }


        public const string FileExtension = ".service-compare";
    }
}
