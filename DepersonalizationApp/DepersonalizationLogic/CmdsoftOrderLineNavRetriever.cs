using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftOrderLineNavRetriever : Base<Guid>
    {
        public CmdsoftOrderLineNavRetriever(SqlConnection sqlConnection, IEnumerable<Guid> orderNavIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select ordNav.cmdsoft_ordernavId");
            sb.AppendLine(" from dbo.cmdsoft_ordernav as ordNav");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ordNav.cmdsoft_navid", orderNavIds);
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