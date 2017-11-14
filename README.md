# NlogForAzure
This Library is to log the exceptions and any kinds of messages into multiple target storage such as Azure Application Insight, Azure Table Storage and Azure Blob.
Nlog library is being used to abstract the logging framework and single library is being used for all kinds of logs.

# NLog with Application Insight

Using NLog for diagnostic tracing in the application, the logs can be sent to Application Insights, where one can explore and search them. The logs cab be merged with the other telemetry coming from the application, so that the traces associated with servicing each user request can be identified, and correlate them with other events and exception reports.
Application Insights NLog Target nuget package adds ApplicationInsights target in your web.config (If the application type that does not have web.config , then install the package but ApplicationInsights needs to be configured programmatically).
•	If configuration is done in NLog though web config then following needs to be done:
// You need this only if you did not define InstrumentationKey in //ApplicationInsights.config
TelemetryConfiguration.Active.InstrumentationKey = "Your_Resource_Key";

Logger logger = LogManager.GetLogger("Example");

logger.Trace("trace log message");
•	If configuration is done in NLog programmatically than create Application Insights target in code and add it to other targets:
var config = new LoggingConfiguration();

ApplicationInsightsTarget target = new ApplicationInsightsTarget();
// You need this only if you did not define InstrumentationKey in ApplicationInsights.config or want to use different instrumentation key
target.InstrumentationKey = "Your_Resource_Key";

LoggingRule rule = new LoggingRule("*", LogLevel.Trace, target);
config.LoggingRules.Add(rule);

LogManager.Configuration = config;

Logger logger = LogManager.GetLogger("Example");

logger.Trace("trace log message");


Can the log be send to other storages as well?

Yes, as Nlog supports multiple targets.

Data that is send through NLog to AppInsight would have Severity level defining the log level.
 There are 7 Event Types in App Insight:
•	Trace
•	Request
•	Page View
•	Custom event
•	Exception
•	Dependency
•	Availability

Based on the requirement of the project these events are used.
Fields supported by telemetry events are provided in the link below:
https://docs.microsoft.com/en-us/azure/application-insights/app-insights-export-data-model
