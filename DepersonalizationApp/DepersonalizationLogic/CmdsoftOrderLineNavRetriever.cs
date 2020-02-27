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
            sb.AppendLine("select orLnNav.cmdsoft_orderlinenavId");
            sb.AppendLine(" from dbo.cmdsoft_orderlinenav as orLnNav");
            sb.AppendLine(" where orLnNav.cmdsoft_orderlinenavId in (select orLnNavIn.cmdsoft_orderlinenavId");
            sb.AppendLine("  from dbo.cmdsoft_orderlinenav as orLnNavIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("orLnNavIn.cmdsoft_orderlinenavId", orderNavIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("orLnNavIn.CreatedOn", "desc", 0, 250);
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