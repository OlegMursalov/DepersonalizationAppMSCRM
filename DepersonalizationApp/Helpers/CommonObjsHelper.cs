using UpdaterApp.Log;

namespace DepersonalizationApp.Helpers
{
    public static class CommonObjsHelper
    {
        public static readonly ILogger Logger = new FileLogger();
    }
}