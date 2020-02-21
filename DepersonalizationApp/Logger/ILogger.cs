using System;

namespace UpdaterApp.Log
{
    public interface ILogger
    {
        void Info(string msg);
        void Error(string msg, Exception ex);
        void Error(string msg);
    }
}