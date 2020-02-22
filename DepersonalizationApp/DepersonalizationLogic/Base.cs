using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using UpdaterApp.Log;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public abstract class Base<T> where T : Entity
    {
        protected IOrganizationService _orgService;
        protected SqlConnection _sqlConnection;
        protected string _retrieveSqlQuery;

        protected ILogger _logger = new FileLogger();

        public Base(IOrganizationService orgService, SqlConnection sqlConnection)
        {
            _orgService = orgService;
            _sqlConnection = sqlConnection;
        }

        protected abstract T ConvertSqlDataReaderToEntity(SqlDataReader sqlReader);

        protected IEnumerable<T> FastRetrieveAll(string query)
        {
            var entities = new List<T>();
            using (var sqlCommand = new SqlCommand())
            {
                sqlCommand.CommandText = query;
                sqlCommand.Connection = _sqlConnection;
                SqlDataReader sqlReader = null;

                try
                {
                    sqlReader = sqlCommand.ExecuteReader();
                }
                catch (Exception ex)
                {
                    _logger.Error("ExecuteReader is crushed", ex);
                }

                if (sqlReader != null && sqlReader.HasRows)
                {
                    while (sqlReader.Read())
                    {
                        try
                        {
                            var entity = ConvertSqlDataReaderToEntity(sqlReader);
                            entities.Add(entity);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Converting SQL column is crushed", ex);
                        }
                    }
                }

                if (sqlCommand != null)
                {
                    sqlCommand.Dispose();
                }
            }
            return entities;
        }
    }
}