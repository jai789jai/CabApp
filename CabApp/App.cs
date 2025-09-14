using CabApp.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp
{
    public class App
    {
        private readonly IAppLogger _appLogger;

        public App(IAppLogger appLogger)
        {
            _appLogger = appLogger;
        }

        public void Run(string[] args)
        {
            try
            {
                _appLogger.LogInfo("Application Started");
                Console.WriteLine("Hello from the other Side.");

            }
            catch(Exception ex)
            {
                _appLogger.LogError("An error occurred", ex);
            }
            finally
            {
                _appLogger.LogInfo("Application Ended");
            }
        }
    }
}
