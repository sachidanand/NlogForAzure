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

#  Nlog with Asp.Net Core

## Steps to Implement NLog with Asp.net Core:

•	Create a new ASP.NET Core project in Visual Studio 2017.
•	Add dependency in csproj manually or using NuGet
Install the latest NLog.Web.AspNetCore.
•	Create a nlog.config file.

<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Warn"
      internalLogFile="c:\temp\internal-nlog.txt">

  <!-- Load the ASP.NET Core plugin -->
  <extensions>
    <add assembly="NLog.Extensions.AzureTableStorage"/>
    <add assembly="NLog.Web.AspNetCore"/>
    <add assembly="Microsoft.ApplicationInsights.NLogTarget" />
  </extensions>

  <!-- the targets to write to -->
  <targets>
    <target name="AzureTableStorage" xsi:type="AzureTableStorage" PartitionKeyPrefixDateFormat="yyyy-MM-dd" LogTimestampFormatString="yyyy-MM-dd hh:mm:ss.000" connectionStringKey="StorageAccountConnectionString" tableName="NLogTableTest" />
    <target name='AI' xsi:type='ApplicationInsightsTarget'  InstrumentationKey="7210f7c7-9afd-42db-95d2-62eb006b9ad4" />
  </targets>

  <!-- rules to map from logger name to target -->
  <rules>
    <!--All logs, including from Microsoft-->
    <logger name="NLogTest.Controllers.HomeController" minlevel="Trace" writeTo="AzureTableStorage" />
    <logger name="NLogTest.Controllers.HomeController" minlevel="Trace" writeTo="AI" />
  </rules>

</nlog>

Since Application Insight and Azure Storage table are used as targets to store data.
Microsoft.ApplicationInsights.NLogTarget dll needs to be added in the project.
NLog.Extensions.AzureTableStorage dll needs to be added in the project.

Now Nlog.Extensions.AzureTableStorage is another project implementing the Nlog in Azure Table Storage. Its reference in the core can be used to send data in the specified Azure Table Storage.

•	 Enable copy to bin folder
•	Update startup.cs
## Add to your startup.cs
using NLog.Extensions.Logging;
using NLog.Web;

public Startup(IHostingEnvironment env)
{
    env.ConfigureNLog("nlog.config");
}

public void ConfigureServices(IServiceCollection Services)
{
    //call this in case you need aspnet-user-authtype/aspnet-user-identity
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
}

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{

    //add NLog to ASP.NET Core
    loggerFactory.AddNLog();

    //add NLog.Web
    app.AddNLogWeb();

   //note: remove the old loggerFactory, like loggerFactory.AddConsole and  loggerFactory.AddDebug

•	Write logs
Inject the ILogger in your controller:
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Index page says hello");
            return View();
        }

##	Output:

When the above code is run the output is reflected in both the defined Azure Storage Table in the form of the row and App Insight as a trace with severity level as information.

# NLog Details
NLog is a free logging platform for .NET, NETSTANDARD, Xamarin, Silverlight and Windows Phone with rich log routing and management capabilities.
 NLog makes it easy to produce and manage high-quality logs for the application regardless of its size or complexity. 
NLog can process diagnostic messages emitted from any .NET language (C#, VB.NET etc.), augment them with contextual information (date and time, severity, thread, process, environment), format according to your preferences and send to one or more targets. 

## Nlog Features
•	NLog is very easy to configure, both through configuration file and programmatically. Even without restarting the application, the configuration can be changed.
•	Every log message can be templated with various layout renders.
•	Even though NLog has targets and pre-defined layouts, you can write custom targets or pass custom values.

## Supported Targets for logging storage

Targets are used to display, store or pass log messages to another destination. NLog can dynamically write to one of multiple targets for each log message.
There are two kinds of target; those that receive and handle the messages, and those that buffer or route the messages to another target. The second group are called 'wrapper' targets.
NLog package 
•	Chainsaw - Sends log messages to the remote instance of Chainsaw application from log4j.
•	ColoredConsole - Writes log messages to the console with customizable coloring.
•	Console - Writes log messages to the console.
•	Database - Writes log messages to the database using an ADO.NET provider.
•	Debug - Mock target - useful for testing.
•	Debugger - Writes log messages to the attached managed debugger.
•	EventLog - Writes log message to the Event Log.
•	File - Writes log messages to one or more files.
•	LogReceiverService - Sends log messages to a NLog Receiver Service (using WCF or Web Services).
•	Mail - Sends log messages by email using SMTP protocol.
•	Memory - Writes log messages to an ArrayList in memory for programmatic retrieval.
•	MethodCall - Calls the specified static method on each log message and passes contextual parameters to it.
•	Network - Sends log messages over the network.
•	NLogViewer - Sends log messages to the remote instance of NLog Viewer.
•	Null - Discards log messages. Used mainly for debugging and benchmarking.
•	OutputDebugString - Outputs log messages through the OutputDebugString() Win32 API.
•	PerfCounter - Increments specified performance counter on each write.
•	Trace - Sends log messages through System.Diagnostics.Trace.
•	WebService - Calls the specified web service on each log message.
Wrappers
•	AsyncWrapper - Provides asynchronous, buffered execution of target writes.
•	AutoFlushWrapper - Causes a flush after each write on a wrapped target.
•	BufferingWrapper - A target that buffers log events and sends them in batches to the wrapped target. Useful in combination with Mail target.
•	FallbackGroup - Provides fallback-on-error.
•	FilteringWrapper - Filters log entries based on a condition.
•	ImpersonatingWrapper - Impersonates another user for the duration of the write.
•	LimitingWrapper - Limits number of log events sent to target.
•	PostFilteringWrapper - Filters buffered log entries based on a set of conditions that are evaluated on a group of events.
•	RandomizeGroup - Sends log messages to a randomly selected target.
•	RepeatingWrapper - Repeats each log event the specified number of times.
•	RetryingWrapper - Retries in case of write error.
•	RoundRobinGroup - Distributes log events to targets in a round-robin fashion.
•	SplitGroup - Writes log events to all targets.
NLog.Extended package  
•	MSMQ - Writes log message to the specified message queue handled by MSMQ.
NLog.Web package  
•	AspNetTrace - Writes log messages to the ASP.NET trace.
Wrappers
•	AspNetBufferingWrapper - Buffers log events for the duration of ASP.NET request and sends them down to the wrapped target at the end of a request.
NLog supports creating custom targets. For more information, see: Extending NLog.

## NLog supports the following platforms: 
NLog supports the following platforms: 
•	.NET Framework 3.5, 4, 4.5 & 4.6
•	Xamarin Android 
•	Xamarin iOs 
•	Windows Phone 8
•	Silverlight 4 and 5
•	Mono 4
•	ASP.NET 4 (NLog.Web package) 
•	ASP.NET Core (NLog.Web.AspNetCore package) 
•	.NET Core (NLog.Extensions.Logging package) 
•	NETSTANDARD - NLog 5 beta


## Working with NLog:
NLog can be downloaded from NuGet.
Just install NLog.Config package and this will install also NLog and NLog.Schema packages - this will result in a starter config and intellisense.
Use the GUI or the following command in the Package Manager Console:
Install-Package NLog.Config

## Creating Log messages:

Two classes that are used : Logger and LogManager, both in the NLog namespace. Logger represents the named source of logs and has methods to emit log messages, and LogManager creates and manages instances of loggers.

Mapping from log sources to outputs is defined separately through Configuration File or Configuration API.

## Creating loggers
It is advised to create one (private static) Logger per class.
namespace MyNamespace
{
  public class MyClass
  {
    private static Logger logger = LogManager.GetCurrentClassLogger();
  }
}
Because loggers are thread-safe, one can simply create the logger once and store it in a static variable.
Log levels
Each log message has associated log level, which identifies how important/detailed the message is. NLog can route log messages based primarily on their logger name and log level.
NLog supports the following log levels:
•	Trace - very detailed logs, which may include high-volume information such as protocol payloads. This log level is typically only enabled during development
•	Debug - debugging information, less detailed than trace, typically not enabled in production environment.
•	Info - information messages, which are normally enabled in production environment
•	Warn - warning messages, typically for non-critical issues, which can be recovered or which are temporary failures
•	Error - error messages - most of the time these are Exceptions 
•	Fatal - very serious errors!
## Writing log messages
In order to emit log message call one of the methods on the Logger. Logger class has six methods whose names correspond to log levels: Trace(), Debug(), Info(), Warn(), Error() and Fatal(). There is also Log() method which takes log level as a parameter.
using NLog;

public class MyClass
{
  private static Logger logger = LogManager.GetCurrentClassLogger();

  public void MyMethod1()
  {
    logger.Trace("Sample trace message");
    logger.Debug("Sample debug message");
    logger.Info("Sample informational message");
    logger.Warn("Sample warning message");
    logger.Error("Sample error message");
    logger.Fatal("Sample fatal error message");

    // alternatively you can call the Log() method
    // and pass log level as the parameter.
    logger.Log(LogLevel.Info, "Sample informational message");
  }

## Trace log level has following overloaded functions:

1.	Logger.Trace Method (Object)
2.	Logger.Trace Method (String)
3.	Logger.Trace<T> Method (T)
4.	Logger.Trace Method (LogMessageGenerator)- LogMessageGenerator is a function returning message to be written. Function is not evaluated if logging is not enabled.
5.	Logger.Trace Method (IFormatProvider, Object)
6.	Logger.Trace<T> Method (IFormatProvider, T)
7.	Logger.Trace Method (String, Boolean)
8.	Logger.Trace Method (String, Byte)
9.	Logger.Trace Method (String, Char)
10.	Logger.Trace Method (String, Decimal)
11.	Logger.Trace Method (String, Double)
12.	Logger.Trace Method (String, Exception)
13.	Logger.Trace Method (String, Int32)
14.	Logger.Trace Method (String, Int64)
15.	Logger.Trace Method (String, Object)
16.	Logger.Trace Method (String,Object[])
17.	Logger.Trace Method (String, SByte)
18.	Logger.Trace Method (String, Single)
19.	Logger.Trace Method (String, String)
20.	Logger.Trace Method (String, UInt32)
21.	Logger.Trace Method (String, UInt64)
22.	Logger.Trace<TArgument> Method (String, TArgument)
23.	Logger.Trace Method (Exception, String,Object[])
24.	Logger.Trace Method (Exception, String,Object[])
25.	Logger.Trace Method (IFormatProvider, String, Byte)
26.	Logger.Trace Method (IFormatProvider, String, Char)
27.	Logger.Trace Method (IFormatProvider, String, Decimal)
28.	Logger.Trace Method (IFormatProvider, String, Double)
29.	Logger.Trace Method (IFormatProvider, String, Int32)
30.	Logger.Trace Method (IFormatProvider, String, Int64)
31.	Logger.Trace Method (IFormatProvider, String, Object)
32.	Logger.Trace Method (IFormatProvider, String, Object[])
33.	Logger.Trace Method (IFormatProvider, String, SByte)
34.	Logger.Trace Method (IFormatProvider, String, Single)
35.	Logger.Trace Method (IFormatProvider, String, String)
36.	Logger.Trace Method (IFormatProvider, String, UInt32)
37.	Logger.Trace Method (IFormatProvider, String, UInt64)
38.	Logger.Trace(TArgument) Method (IFormatProvider, String, TArgument)
39.	Logger.Trace Method (String, Object, Object)
40.	Logger.Trace(TArgument1, TArgument2) Method (String, TArgument1, TArgument2)
41.	Logger.Trace Method (Exception, IFormatProvider, String, Object[])
42.	Logger.Trace(TArgument1, TArgument2) Method (IFormatProvider, String, TArgument1, TArgument2)
43.	Logger.Trace Method (String, Object, Object, Object)
44.	Logger.Trace(TArgument1, TArgument2, TArgument3) Method (String, TArgument1, TArgument2, TArgument3)
45.	Logger.Trace(TArgument1, TArgument2, TArgument3) Method (IFormatProvider, String, TArgument1, TArgument2, TArgument3)

## For more details, refer 

 http://nlog-project.org/documentation/v4.3.0/html/T_NLog_Logger.htm#! 

## Details for NLog and .Net Core are provided in the link below:

https://github.com/NLog/NLog.Extensions.Logging/blob/master/README.md

## References:

http://nlog-project.org/

https://github.com/nlog/nlog/wiki

https://github.com/NLog/NLog.Extensions.Logging/blob/master/README.md

https://github.com/nlog/nlog/wiki/platform-support

https://docs.microsoft.com/en-us/dotnet/standard/net-standard




