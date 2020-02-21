using Microsoft.Xrm.Sdk;
using System;

namespace UpdaterApp.LogicOfConnection
{
    public class CRMConnectState
    {
        public IOrganizationService OrganizationService { get; }
        public bool IsConnect { get; }
        public Exception ConnectException { get; }

        public CRMConnectState(bool isConnect, IOrganizationService organizationService, Exception connectException)
        {
            IsConnect = isConnect;
            OrganizationService = organizationService;
            ConnectException = connectException;
        }
    }
}