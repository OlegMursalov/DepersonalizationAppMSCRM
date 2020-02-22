using System;

namespace DepersonalizationApp.LogicOfConnection
{
    public interface IConnector : IDisposable
    {
        IConnectionState GetConnectState();

        void Execute();
    }
}