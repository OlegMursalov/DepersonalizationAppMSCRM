using UpdaterApp.Log;

namespace DepersonalizationApp.Helpers
{
    public static class CommonObjsHelper
    {
        public static readonly ILogger Logger = new FileLogger();

        /// <summary>
        /// +ХХ ХХХ ХХХ-ХХ-ХХ
        /// </summary>
        public static readonly string TelephoneMask1 = "+XX XXX XXX-XX-XX";

        /// <summary>
        /// XXXXX@XXXXX.XX
        /// </summary>
        public static readonly string EmailMask1 = "XXXXX@XXXXX.XX";
    }
}