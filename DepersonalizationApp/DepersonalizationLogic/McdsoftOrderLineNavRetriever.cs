using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class McdsoftOrderLineNavRetriever : Base<Guid>
    {
        public McdsoftOrderLineNavRetriever(SqlConnection sqlConnection, IEnumerable<Guid> orderNavIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select orLnNav.cmdsoft_orderlinenavId");
            sb.AppendLine(" from dbo.mcdsoft_orderlinenav as orLnNav");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("orLnNav.mcdsoft_navid", orderNavIds);
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