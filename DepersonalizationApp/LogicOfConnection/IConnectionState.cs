using System;

namespace DepersonalizationApp.LogicOfConnection
{
    public interface IConnectionState
    {
        bool IsConnect { get; }
        Exception Exception { get; }
    }
}