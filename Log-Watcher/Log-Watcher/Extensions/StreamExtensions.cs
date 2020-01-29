using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log_Watcher
{
    public static class StreamExtensions
    {
        public static long LineCount(this StreamReader sr)
        {
            int result = 0;
            while (sr.ReadLine() != null) { result++; }
            sr.BaseStream.Position = 0;
            return result;
        }
    }
}
