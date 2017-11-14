using System;

namespace NLog.Target.Custom
{
    public class TableStorage : Common
    {
        public TableStorage()
        {
        }
        public TableStorage(LogEventInfo logEvent, string layoutMessage) : base(logEvent, layoutMessage)
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = DateTime.Now.ToString("") + "_" + LoggerName;
        }
    }
}


