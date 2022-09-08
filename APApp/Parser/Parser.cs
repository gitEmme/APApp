using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace APApp.Parser
{
    class Parser
    {
        private static Dictionary<string, int> wordCountsDict = new Dictionary<string, int>();
        private static List<WordCount> list = new List<WordCount>();

        public class WordCount
        {
            public string word;
            public int count;

            public WordCount(string word, int n)
            {
                this.word = word;
                this.count = n;
            }
        }

        public static async Task<List<WordCount>> WordCountMapAsync(string filePath, IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            ProgressReportModel report = new ProgressReportModel();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            List<WordCount>  list = new List<WordCount>();

            report.Phase = "Map step";
            //var readTask = reader.ReadToEndAsync();
            var readTask = Task.Run(async () =>
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] words = line.Split(' ');
                    foreach (string w in words)
                    {
                        list.Add(new WordCount(w, 1));
                    }                  
                }
                return list;
            });

            var progressTask = Task.Run(async () =>
            {
                while (stream.Position < stream.Length)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    cancellationToken.ThrowIfCancellationRequested();
                    report.PercentageComplete = (int)((stream.Position * 100) / stream.Length);
                    progress.Report(report);
                }
            });

            await Task.WhenAll(readTask, progressTask);

            return readTask.Result;
        }

        public async Task<Dictionary<string, int>> WordCountReduceAsync(string filepath, IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            ProgressReportModel report = new ProgressReportModel();

            Dictionary<string, int>  dict = new Dictionary<string, int>();
           
            string[] words;
            Debug.WriteLine("WordCountMapReduce");
            var mapreduceTask = Task.Run(async () =>
            {
                int i = 0;
                report.Phase = "Reduce step";
                // reduce phase
                Console.WriteLine("Word List length: " + list.Count);
                foreach (WordCount item in list)
                {
                    i++;
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                    cancellationToken.ThrowIfCancellationRequested();
                    if (dict.ContainsKey(item.word) == false)
                        dict.Add(item.word, item.count);
                    else
                        ++dict[item.word];

                    report.WordCountDict = dict;
                    report.PercentageComplete = i * 100 / list.Count;
                    progress.Report(report);
                }
                return dict;
            });

            await Task.WhenAll(mapreduceTask);

            return mapreduceTask.Result;

        }

        public async Task<Dictionary<string, int>> SortWordCountDict(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            Debug.WriteLine("SortWordCountDict");
            var sortTask = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                ProgressReportModel report = new ProgressReportModel();
                report.Phase = "Sorting dictionary";
                report.PercentageComplete = 0;
                progress.Report(report);
                cancellationToken.ThrowIfCancellationRequested();
                return Parser.wordCountsDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            });

            await Task.WhenAll(sortTask);
            return sortTask.Result;
        }

        public async Task<Dictionary<string, int>> BuildDictionaryAsync(string filepath, IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            Debug.WriteLine("BuildDictionaryAsync");
            var dictTask = Task.Run(async () =>
            {
                list = await Parser.WordCountMapAsync(filepath, progress, cancellationToken);
                Parser.wordCountsDict = await this.WordCountReduceAsync(filepath, progress, cancellationToken);
                // sort result
                Parser.wordCountsDict = await this.SortWordCountDict(progress, cancellationToken);
                return Parser.wordCountsDict;
            });

            await Task.WhenAll(dictTask);
            return dictTask.Result;
        }

        public static Dictionary<string, int> GetSortedWordCountDictionary()
        {
            return Parser.wordCountsDict;
        }

    }
}
