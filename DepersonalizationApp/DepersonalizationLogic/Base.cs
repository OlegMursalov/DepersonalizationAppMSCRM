using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UpdaterApp.Log;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public abstract class Base<T>
    {
        protected IOrganizationService _orgService;
        protected SqlConnection _sqlConnection;
        protected string _retrieveSqlQuery;

        protected ILogger _logger = CommonObjsHelper.Logger;

        public Base(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public Base(IOrganizationService orgService, SqlConnection sqlConnection)
        {
            _orgService = orgService;
            _sqlConnection = sqlConnection;
        }

        protected abstract T ConvertSqlDataReaderItem(SqlDataReader sqlReader);

        protected IEnumerable<T> ExecuteRetrieveAllItems(string sqlQuery)
        {
            var items = new List<T>();
            using (var sqlCommand = new SqlCommand())
            {
                sqlCommand.CommandText = sqlQuery;
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
                            var item = ConvertSqlDataReaderItem(sqlReader);
                            items.Add(item);
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
            return items;
        }

        protected IEnumerable<T> FastRetrieveAllItems()
        {
            var sqlQuery = _retrieveSqlQuery;
            var allItems = new List<T>();
            if (sqlQuery.IndexOf("offset") != -1 && sqlQuery.IndexOf("fetch") != -1)
            {
                while (true)
                {
                    var items = ExecuteRetrieveAllItems(sqlQuery);
                    allItems.AddRange(items);
                    if (items.Count() > 0)
                    {
                        try
                        {
                            var offsetNumber = SqlQueryHelper.GetOffsetNumber(sqlQuery);
                            var fetchNumber = SqlQueryHelper.GetFetchNumber(sqlQuery);
                            sqlQuery = SqlQueryHelper.ChangeSqlQueryPagination(sqlQuery, offsetNumber + fetchNumber, fetchNumber);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("FastRetrieveAllItems - error when SqlQueryHelper tried change sqlQuery", ex);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                var items = ExecuteRetrieveAllItems(sqlQuery);
                allItems.AddRange(items);
            }
            return allItems;
        }
    }
}