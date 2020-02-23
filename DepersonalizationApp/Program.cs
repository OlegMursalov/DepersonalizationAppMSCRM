using CRMEntities;
using DepersonalizationApp.LogicOfConnection;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

                            var opportunityUpdater = new OpportunityUpdater(orgService, sqlConnection);
                            opportunityUpdater.Process();

                        }
                    }
                }
            }
        }
    }
}