using UpdaterApp.Log;
using UpdaterApp.LogicOfUpdater;
using System.Configuration;
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

                // Обезличивание проектов
                var opportunityUpdater = new OpportunityUpdater(organizationService, amountOfLastCreated: 1000);
                opportunityUpdater.Process();
            }
        }
    }
}