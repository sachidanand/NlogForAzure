using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace NLogTestWebApplication.Controllers
{
  
    public class HomeController : Controller
    {

        static Logger logger = LogManager.GetLogger("Web Application");
        public ActionResult Index()
        {
            LogEventInfo theEvent = new LogEventInfo(LogLevel.Trace, logger.Name, "Web Application Simple Trace");
            theEvent.Properties["CustomField1"] = "CF1";
            theEvent.Properties["CustomField2"] = "CF2";
            theEvent.Properties["CustomField3"] = "CF3";
            theEvent.Properties["CustomField4"] = "CF4";
            theEvent.Properties["CustomField5"] = "CF5";
            logger.Log(theEvent);
            // logger.Trace("Web Application Simple Trace");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}