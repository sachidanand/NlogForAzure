using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLogTestApplication
{
    class Program
    {
        static Logger logger = LogManager.GetLogger("Program");
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start");
                logger.Trace("Simple trace");
                Console.WriteLine("End");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
