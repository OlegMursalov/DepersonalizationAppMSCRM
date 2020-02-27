using DepersonalizationApp.DepersonalizationLogic;
using DepersonalizationApp.LogicOfConnection;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using System.Linq;
using UpdaterApp.DepersonalizationLogic;
using UpdaterApp.LogicOfConnection;

namespace UpdaterApp
{
    public class Program
    {
        public static void Main()
        {
            var userName = ConfigurationManager.AppSettings.Get("userName");
            var password = ConfigurationManager.AppSettings.Get("password");
            var soapOrgServiceUri = ConfigurationManager.AppSettings.Get("soapOrgServiceUri");
            var sqlConnectionString = ConfigurationManager.AppSettings.Get("sqlConnectionString");
            using (var crmConnector = new CRMConnector(userName, password, soapOrgServiceUri))
            {
                crmConnector.Execute();
                var crmConnectionState = (CRMConnectionState)crmConnector.GetConnectState();
                if (crmConnectionState.IsConnect)
                {
                    using (var sqlConnector = new SQLConnector(sqlConnectionString))
                    {
                        sqlConnector.Execute();
                        var sqlConnectionState = (SQLConnectionState)sqlConnector.GetConnectState();
                        if (sqlConnectionState.IsConnect)
                        {
                            var orgService = (IOrganizationService)crmConnectionState.Proxy;
                            var sqlConnection = sqlConnectionState.SqlConnection;

                            // Извлечение
                            var retriever = new Retriever(sqlConnection);
                            var allRetrieved = retriever.Execute();

                            // Обновление
                            var updater = new Updater(orgService, sqlConnection);
                            updater.Execute(allRetrieved);

                            // Удаление
                            var deleter = new Deleter(orgService, sqlConnection);
                            deleter.Execute(allUpdated);
                        }
                    }
                }
            }
        }
    }
}