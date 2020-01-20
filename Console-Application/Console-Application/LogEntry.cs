using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Application
{
    public sealed class LogEntry
    {
        private DateTime dateTimeStamp = default;
        public string DateTimeStamp
        {
            get { return dateTimeStamp.ToString(); }
            set { DateTime.TryParse(value, out dateTimeStamp); }
        }
        private LogType type;
        public string Type
        {
            get { return type.ToFriendlyString(); }
            set { Enum.TryParse(value, out type); }
        }
        public string Content { get; set; }
        public LogEntry(DateTime _dateTimeStamp, LogType _type, string content)
        {
            dateTimeStamp = _dateTimeStamp;
            type = _type;
            Content = content;
        }
    }
}
