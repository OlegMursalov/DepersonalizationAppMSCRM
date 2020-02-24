using DepersonalizationApp.Helpers;
using DepersonalizationApp.LogicOfConnection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Net;
using System.ServiceModel.Description;
using UpdaterApp.Log;

namespace UpdaterApp.LogicOfConnection
{
    public class CRMConnector : IConnector
    {
        private readonly ILogger _logger = CommonObjsHelper.Logger;

        private string _userName;
        private string _password;
        private string _soapOrgServiceUri;
        private CRMConnectionState _connectState;

        public CRMConnector(string userName, string password, string soapOrgServiceUri)
        {
            _userName = userName;
            _password = password;
            _soapOrgServiceUri = soapOrgServiceUri;
        }

        public IConnectionState GetConnectState()
        {
            return _connectState;
        }

        public void Execute()
        {
            if (_connectState == null)
            {
                bool isConnect = false;
                Exception connectException = null;
                OrganizationServiceProxy proxy = null;
                try
                {
                    var credentials = new ClientCredentials();
                    credentials.Windows.ClientCredential = new NetworkCredential(_userName, _password);
                    var serviceUri = new Uri(_soapOrgServiceUri);
                    proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
                    proxy.EnableProxyTypes();
                    var response = (WhoAmIResponse)proxy.Execute(new WhoAmIRequest());
                    isConnect = true;
                    _logger.Info($"User '{response.UserId}' opened CRM session for organization '{response.OrganizationId}', connect is succesful");
                }
                catch (Exception ex)
                {
                    connectException = ex;
                    _logger.Error($"CRM connection is failed", ex);
                }
                _connectState = new CRMConnectionState(isConnect, proxy, connectException);
            }
        }

        public void Dispose()
        {
            if (_connectState != null && _connectState.Proxy != null)
            {
                _connectState.Proxy.Dispose();
            }
        }
    }
}