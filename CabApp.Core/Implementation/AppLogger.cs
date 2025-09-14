using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation
{
    public class AppLogger : Abstraction.IAppLogger
    {
        private readonly Logger Logger;
        public AppLogger()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }
    
        public void LogInfo(string message)
        {
            var logEventInfo = LogEventInfo.Create(NLog.LogLevel.Info, "AppLogger", message);
            Logger.Log(typeof(AppLogger), logEventInfo);
        }

        public void LogError(string message, Exception ex)
        {
            var logEventInfo = LogEventInfo.Create(NLog.LogLevel.Error, "AppLogger", message);
            Logger.Log(typeof(AppLogger), logEventInfo);
        }

    }
}
