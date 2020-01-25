using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log_Watcher
{
    public sealed class SavedLog
    {
        public string Path { get; set; }
        public string Alias { get; set; }
        public SavedLog(string path, string alias)
        {
            Path = path;
            Alias = alias;
        }
    }
}
