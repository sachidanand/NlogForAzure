
using Microsoft.ApplicationInsights;
using NLog.Targets;
using Microsoft.ApplicationInsights.DataContracts;
using System.ComponentModel.DataAnnotations;

namespace NLog.Target.Custom
{
    [Target("AzureAppInsight")]
    public sealed class NLogAppInsight : TargetWithLayout
    {
        private TelemetryClient telemetryClient;

        [Required]
        public string InstrumentationKey { get; set; }

        internal TelemetryClient TelemetryClient
        {
            get { return this.telemetryClient; }
        }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            this.telemetryClient = new TelemetryClient();
            if (!string.IsNullOrEmpty(this.InstrumentationKey))
            {
                this.telemetryClient.InstrumentationKey = this.InstrumentationKey;
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent.Exception != null)
            {
                this.SendException(logEvent);
            }
            else
            {
                this.SendTrace(logEvent);
            }
        }

        private void SendException(LogEventInfo logEvent)
        {
            var exceptionTelemetry = new ExceptionTelemetry(logEvent.Exception)
            {
                SeverityLevel = this.GetSeverityLevel(logEvent.Level)
            };

            string logMessage = this.Layout.Render(logEvent);
            Common common = new Common(logEvent, logMessage);
            exceptionTelemetry.Properties.Add("LoggerName", common.LoggerName);
            exceptionTelemetry.Properties.Add("Level", common.Level.ToString());
            exceptionTelemetry.Properties.Add("LogTimeStamp", common.LogTimeStamp);
            exceptionTelemetry.Properties.Add("MachineName", common.MachineName);
            exceptionTelemetry.Properties.Add("Message", common.Message);
            exceptionTelemetry.Properties.Add("MessageWithLayout", common.MessageWithLayout);
            this.telemetryClient.Track(exceptionTelemetry);
        }

        private void SendTrace(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);

            var trace = new TraceTelemetry(logMessage)
            {
                SeverityLevel = this.GetSeverityLevel(logEvent.Level)
            };
            Common common = new Common(logEvent, logMessage);
            trace.Properties.Add("LoggerName", common.LoggerName);
            trace.Properties.Add("Level", common.Level.ToString());
            trace.Properties.Add("LogTimeStamp", common.LogTimeStamp);
            trace.Properties.Add("MachineName", common.MachineName);
            trace.Properties.Add("Message", common.Message);
            trace.Properties.Add("MessageWithLayout", common.MessageWithLayout);
            trace.Properties.Add("CustomField1", common.CustomField1);
            trace.Properties.Add("CustomField2", common.CustomField2);
            trace.Properties.Add("CustomField3", common.CustomField3);
            trace.Properties.Add("CustomField4", common.CustomField4);
            trace.Properties.Add("CustomField5", common.CustomField5);
            this.telemetryClient.Track(trace);
        }

        private SeverityLevel? GetSeverityLevel(LogLevel logEventLevel)
        {
            if (logEventLevel == null)
            {
                return null;
            }

            if (logEventLevel.Ordinal == LogLevel.Trace.Ordinal ||
                logEventLevel.Ordinal == LogLevel.Debug.Ordinal)
            {
                return SeverityLevel.Verbose;
            }

            if (logEventLevel.Ordinal == LogLevel.Info.Ordinal)
            {
                return SeverityLevel.Information;
            }

            if (logEventLevel.Ordinal == LogLevel.Warn.Ordinal)
            {
                return SeverityLevel.Warning;
            }

            if (logEventLevel.Ordinal == LogLevel.Error.Ordinal)
            {
                return SeverityLevel.Error;
            }

            if (logEventLevel.Ordinal == LogLevel.Fatal.Ordinal)
            {
                return SeverityLevel.Critical;
            }

            // The only possible value left if OFF but we should never get here in this case
            return null;
        }
    }
}

