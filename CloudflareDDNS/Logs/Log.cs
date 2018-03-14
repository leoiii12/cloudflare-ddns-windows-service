using System;
using System.Globalization;
using System.IO;

namespace CloudflareDDNS.Logs
{
    public class Log : ILog
    {
        private readonly string _pathToLog;

        public Log(string programmeName)
        {
            var directoryInfo = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), programmeName));
            
            _pathToLog = Path.Combine(directoryInfo.FullName, "log.txt");
        }

        public void WriteLine(string line)
        {
            using (var file = new StreamWriter(_pathToLog, true))
            {
                file.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                file.WriteLine(line);
            }
        }

        public void WriteException(Exception exception)
        {
            using (var file = new StreamWriter(_pathToLog, true))
            {
                file.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                file.WriteLine(exception.GetBaseException().Message);
                file.WriteLine(exception.StackTrace);
            }
        }
    }
}