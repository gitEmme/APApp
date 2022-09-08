using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APApp
{
    public class ProgressReportModel
    {
        public string Phase { get; set; }
        public int PercentageComplete { get; set; } = 0;
        public Dictionary<string, int> WordCountDict { get; set; } = new Dictionary<string, int>();
        
    }
}
