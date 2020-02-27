using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class McdsoftOrderLineNavLink
    {
        public Guid Id { get; set; }

        // Сервисное обращение (mcdsoft_sales_appeal)
        public Guid? CmdsoftRefOrderlineNav { get; set; }
    }

    public class McdsoftOrderLineNavRetriever : Base<McdsoftOrderLineNavLink>
    {
        public McdsoftOrderLineNavRetriever(SqlConnection sqlConnection, IEnumerable<Guid> mcdsoftOrderLineNavIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select orLnNav.mcdsoft_orderlinenavId, orLnNav.cmdsoft_ref_orderlinenav");
            sb.AppendLine(" from dbo.mcdsoft_orderlinenav as orLnNav");
            sb.AppendLine(" where orLnNav.mcdsoft_orderlinenavId in (select orLnNavIn.mcdsoft_orderlinenavId");
            sb.AppendLine("  from dbo.mcdsoft_orderlinenav as orLnNavIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("orLnNavIn.cmdsoft_ref_navid", mcdsoftOrderLineNavIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("orLnNavIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        public IEnumerable<McdsoftOrderLineNavLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override McdsoftOrderLineNavLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new McdsoftOrderLineNavLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                CmdsoftRefOrderlineNav = sqlReader.GetValue(0) as Guid?
            };
        }
    }
}