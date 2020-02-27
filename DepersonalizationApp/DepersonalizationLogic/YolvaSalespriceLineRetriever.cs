using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class YolvaSalespriceLineRetriever : Base<Guid>
    {
        public YolvaSalespriceLineRetriever(SqlConnection sqlConnection, IEnumerable<Guid> yolvaSalesPriceIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select ySlPrcLine.yolva_salespricelineId");
            sb.AppendLine(" from yolva_salespriceline as ySlPrcLine");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ySlPrcLine.yolva_salespriceid", yolvaSalesPriceIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }
        
        public IEnumerable<Guid> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override Guid ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return (Guid)sqlReader.GetValue(0);
        }
    }
}