using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Net;
using System.ServiceModel.Description;
using UpdaterApp.Log;

namespace UpdaterApp.LogicOfConnection
{
    public class CRMConnector
    {
        private readonly ILogger _logger = new FileLogger();

        private string _userName;
        private string _password;
        private string _soapOrgServiceUri;
        private CRMConnectState _connectState;

        public CRMConnector(string userName, string password, string soapOrgServiceUri)
        {
            _userName = userName;
            _password = password;
            _soapOrgServiceUri = soapOrgServiceUri;
        }

        public CRMConnectState GetConnectState()
        {
            return _connectState;
        }

        public void Execute()
        {
            if (_connectState == null)
            {
                IOrganizationService organizationService = null;
                bool isConnect = false;
                Exception connectException = null;
                try
                {
                    var credentials = new ClientCredentials();
                    credentials.Windows.ClientCredential = new NetworkCredential(_userName, _password);
                    var serviceUri = new Uri(_soapOrgServiceUri);
                    using (var proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null))
                    {
                        proxy.EnableProxyTypes();
                        var response = (WhoAmIResponse)proxy.Execute(new WhoAmIRequest());
                        organizationService = proxy;
                        isConnect = true;
                        // _logger.Info($"User '{response.UserId}' opened session for organization '{response.OrganizationId}', connect is succesful");
                    }
                }
                catch (Exception ex)
                {
                    connectException = ex;
                    _logger.Error($"Connection is failed", ex);
                }
                _connectState = new CRMConnectState(isConnect, organizationService, connectException);
            }
        }
    }
}