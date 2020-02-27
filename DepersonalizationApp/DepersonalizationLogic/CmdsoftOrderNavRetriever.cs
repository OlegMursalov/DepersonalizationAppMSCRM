using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftOrderNavRetriever : Base<Guid>
    {
        public CmdsoftOrderNavRetriever(SqlConnection sqlConnection, IEnumerable<Guid> opprotunityIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select ordNav.cmdsoft_ordernavId");
            sb.AppendLine(" from dbo.cmdsoft_ordernav as ordNav");
            sb.AppendLine(" where ordNav.cmdsoft_ordernavId in (select ordNavIn.cmdsoft_ordernavId");
            sb.AppendLine("  from dbo.cmdsoft_ordernav as ordNavIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ordNavIn.cmdsoft_ordernavId", opprotunityIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("ordNavIn.CreatedOn", "desc", 0, 500);
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