using CRMEntities;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Configuration;
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
            var crmConnector = new CRMConnector(userName, password, soapOrgServiceUri);
            crmConnector.Execute();
            var connectState = crmConnector.GetConnectState();
            if (connectState.IsConnect)
            {
                var organizationService = connectState.OrganizationService;
                using (var serviceContext = new OrganizationServiceCtx(organizationService))
                {
                    // Обезличивание проектов
                    var opportunityUpdater = new OpportunityUpdater(serviceContext);
                    opportunityUpdater.Process();
                }
            }
        }
    }
}