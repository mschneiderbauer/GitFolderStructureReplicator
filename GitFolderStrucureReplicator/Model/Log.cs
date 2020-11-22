using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitFolderStructureReplicator
{
    public class Log
    {
        public string Path { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public override string ToString()
        {
            return $"{DateTime}{Environment.NewLine}{Path}{Environment.NewLine}{Message}";
        }

        public static string GetText(List<Log> logs)
        {
            StringBuilder text = new StringBuilder();
            foreach (var log in logs)
            {
                text.Append(log.ToString()).Append(Environment.NewLine);
            }
            return text.ToString();
        }
    }
}
