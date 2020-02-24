using DepersonalizationApp.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using UpdaterApp.Log;

namespace DepersonalizationApp.LogicOfConnection
{
    public class SQLConnector : IConnector
    {
        private readonly ILogger _logger = CommonObjsHelper.Logger;

        private string _connectionString;
        private SQLConnectionState _connectState;

        public SQLConnector(string connectionString)
        {
            _connectionString = connectionString;
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
                SqlConnection connection = null;
                Exception connectException = null;
                try
                {
                    connection = new SqlConnection(_connectionString);
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        isConnect = true;
                        _logger.Info($"Opened SQL session for connection string '{_connectionString}'");
                    }
                    else
                    {
                        throw new Exception($"SQL connection is [{connection.State}]");
                    }
                }
                catch (Exception ex)
                {
                    connectException = ex;
                    _logger.Error($"SQL connection is failed", ex);
                }
                _connectState = new SQLConnectionState(isConnect, connection, connectException);
            }
        }

        public void Dispose()
        {
            if (_connectState != null && _connectState.SqlConnection != null)
            {
                if (_connectState.SqlConnection.State != ConnectionState.Closed)
                {
                    _connectState.SqlConnection.Close();
                }
                _connectState.SqlConnection.Dispose();
            }
        }
    }
}