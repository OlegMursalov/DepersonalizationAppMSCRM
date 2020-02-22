using System;
using System.Data.SqlClient;

namespace DepersonalizationApp.LogicOfConnection
{
    public class SQLConnectionState : IConnectionState
    {
        public bool IsConnect { get; }
        public SqlConnection SqlConnection { get; }
        public Exception Exception { get; }

        public SQLConnectionState(bool isConnect, SqlConnection sqlConnection, Exception exception)
        {
            IsConnect = isConnect;
            SqlConnection = sqlConnection;
            Exception = exception;
        }
    }
}