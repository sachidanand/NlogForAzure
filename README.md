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

Steps to Implement NLog with Asp.net Core:

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
Add to your startup.cs
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

•	Output:

When the above code is run the output is reflected in both the defined Azure Storage Table in the form of the row and App Insight as a trace with severity level as information.

