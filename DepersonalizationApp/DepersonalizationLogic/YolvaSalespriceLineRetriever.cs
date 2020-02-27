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
            sb.AppendLine(" where ySlPrcLine.yolva_salespricelineId in (select ySlPrcLineIn.yolva_salespricelineId");
            sb.AppendLine("  from dbo.yolva_salespriceline as ySlPrcLineIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ySlPrcLineIn.yolva_salespriceid", yolvaSalesPriceIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("ySlPrcLineIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
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