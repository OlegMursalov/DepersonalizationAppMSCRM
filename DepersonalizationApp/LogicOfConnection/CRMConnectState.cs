using DepersonalizationApp.LogicOfConnection;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace UpdaterApp.LogicOfConnection
{
    public class CRMConnectionState : IConnectionState
    {
        public OrganizationServiceProxy Proxy { get; }
        public bool IsConnect { get; }
        public Exception Exception { get; }

        public CRMConnectionState(bool isConnect, OrganizationServiceProxy proxy, Exception exception)
        {
            IsConnect = isConnect;
            Proxy = proxy;
            Exception = exception;
        }
    }
}