using log4net;
using log4net.Config;
using System;

namespace UpdaterApp.Log
{
    public class FileLogger : ILogger
    {
        private ILog _log;

        public FileLogger()
        {
            _log = LogManager.GetLogger("LOGGER");
            XmlConfigurator.Configure();
        }

        public void Error(string msg, Exception ex)
        {
            _log.Error(msg, ex);
        }

        public void Info(string msg)
        {
            _log.Info(msg);
        }

        public void CommonWrapLog(Action action)
        {
            var methodName = action.Method.Name;
            try
            {
                action();
                _log.Info(methodName);
            }
            catch (Exception ex)
            {
                _log.Error(methodName, ex);
            }
        }

        public void Error(string msg)
        {
            _log.Error(msg);
        }
    }
}