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
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ordNav.cmdsoft_navid", opprotunityIds);
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