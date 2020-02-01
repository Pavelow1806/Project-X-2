

namespace Log_Watcher
{
    public sealed class LogItem
    {
        public string Line { get; set; } = "";
        public LogItem(string line)
        {
            Line = line;

        }
    }
}
