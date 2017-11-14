using System;
using System.Text;
using System.Collections;
using Microsoft.WindowsAzure.Storage.Table;

namespace NLog.Target.Custom
{
    
    public class Common : TableEntity
    {
        public string LogTimeStamp { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public string MessageWithLayout { get; set; }
        public string ExceptionData { get; set; }
        public string MachineName { get; set; }

        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }

        private readonly object _syncRoot = new object();

        public Common()
        {
        }
        public Common(LogEventInfo logEvent, string layoutMessage)
        {
            lock (_syncRoot)
            {
                LoggerName = logEvent.LoggerName;
                Level = logEvent.Level.Name;
                Message = logEvent.FormattedMessage;
                MessageWithLayout = layoutMessage;
                if (logEvent.Exception != null)
                {
                    Exception = logEvent.Exception.ToString();
                    if (logEvent.Exception.Data.Count > 0)
                    {
                        ExceptionData = GetExceptionDataAsString(logEvent.Exception);
                    }
                    if (logEvent.Exception.InnerException != null)
                    {
                        InnerException = logEvent.Exception.InnerException.ToString();
                    }
                }
                if (logEvent.StackTrace != null)
                {
                    StackTrace = logEvent.StackTrace.ToString();
                }

                MachineName = Environment.MachineName;
                LogTimeStamp = DateTime.Now.ToString();
                CustomField1 = logEvent.Properties["CustomField1"].ToString();
                CustomField2 = logEvent.Properties["CustomField2"].ToString();
                CustomField3 = logEvent.Properties["CustomField3"].ToString();
                CustomField4 = logEvent.Properties["CustomField4"].ToString();
                CustomField5 = logEvent.Properties["CustomField5"].ToString();
            }

        }

        private static string GetExceptionDataAsString(Exception exception)
        {
            var data = new StringBuilder();
            foreach (DictionaryEntry entry in exception.Data)
            {
                data.AppendLine(entry.Key + "=" + entry.Value);
            }
            return data.ToString();
        }

    }



}

