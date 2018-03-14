using System;

namespace CloudflareDDNS.Logs
{
    public interface ILog
    {
        void WriteLine(string line);
        void WriteException(Exception exception);
    }
}