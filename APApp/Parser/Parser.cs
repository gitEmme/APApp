using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace APApp.Parser
{
    class Parser
    {
        private static Dictionary<string, int> wordCountsDict = new Dictionary<string, int>();
        private List<WordCount> list = new List<WordCount>();

        private class WordCount
        {
            public string word;
            public int count;

            public WordCount(string word, int n)
            {
                this.word = word;
                this.count = n;
            }
        }

        private async Task<Dictionary<string, int>> WordCountMapReduce(string filepath)
        {
            Dictionary<string, int>  dict = new Dictionary<string, int>();
            list = new List<WordCount>();
            StreamReader streamReader = new StreamReader(filepath);
            string line;
            string[] words;
            // read file
            while ((line = streamReader.ReadLine()) != null)
            {
                words = line.Split(' ');
                // map phase
                foreach (string w in words)
                    this.list.Add(new WordCount(w, 1));
            }
            streamReader.Close();

            // reduce phase
            foreach (WordCount item in this.list)
            {
                if (dict.ContainsKey(item.word) == false)
                    dict.Add(item.word, item.count);
                else
                    ++dict[item.word];
            }

            return dict;

        }

        private async Task<Dictionary<string, int>> SortWordCountDict()
        {
           return Parser.wordCountsDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<Dictionary<string, int>> BuildDictionaryAsync(string filepath)
        {
            Parser.wordCountsDict = await this.WordCountMapReduce(filepath);
            // sort result
            Parser.wordCountsDict =  await this.SortWordCountDict();
            return Parser.wordCountsDict;
        }

        public static Dictionary<string, int> GetSortedWordCountDictionary()
        {
            return Parser.wordCountsDict;
        }
    }
}
