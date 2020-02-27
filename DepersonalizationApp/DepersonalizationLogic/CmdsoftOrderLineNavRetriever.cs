using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftOrderLineNavLink
    {
        public Guid Id { get; set; }

        // Сервисное обращение (mcdsoft_sales_appeal)
        public Guid? McdsoftRefOrderlineNav { get; set; }
    }

    public class CmdsoftOrderLineNavRetriever : Base<CmdsoftOrderLineNavLink>
    {
        public CmdsoftOrderLineNavRetriever(SqlConnection sqlConnection, IEnumerable<McdsoftSalesAppealLink> mcdsoftSalesAppealLinks) : base(sqlConnection)
        {
            var cmdsoftOrderLineNavIds = new List<Guid>();
            foreach (var mcdsoftSalesAppealLink in mcdsoftSalesAppealLinks)
            {
                if (mcdsoftSalesAppealLink.CmdsoftRefOrderlinenav != null)
                {
                    cmdsoftOrderLineNavIds.Add(mcdsoftSalesAppealLink.CmdsoftRefOrderlinenav.Value);
                }
            }
            var cmdsoftOrderLineNavIdsDistinct = cmdsoftOrderLineNavIds.Distinct();
            _retrieveSqlQuery = SetQuery(cmdsoftOrderLineNavIdsDistinct);
        }

        public CmdsoftOrderLineNavRetriever(SqlConnection sqlConnection, IEnumerable<Guid> cmdsoftOrderLineNavIds) : base(sqlConnection)
        {
            _retrieveSqlQuery = SetQuery(cmdsoftOrderLineNavIds);
        }

        private string SetQuery(IEnumerable<Guid> cmdsoftOrderLineNavIds)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select orLnNav.cmdsoft_orderlinenavId, orLnNav.mcdsoft_ref_orderlinenav");
            sb.AppendLine(" from dbo.cmdsoft_orderlinenav as orLnNav");
            sb.AppendLine(" where orLnNav.cmdsoft_orderlinenavId in (select orLnNavIn.cmdsoft_orderlinenavId");
            sb.AppendLine("  from dbo.cmdsoft_orderlinenav as orLnNavIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("orLnNavIn.cmdsoft_orderlinenavId", cmdsoftOrderLineNavIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("orLnNavIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            return sb.ToString();
        }

        public IEnumerable<CmdsoftOrderLineNavLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override CmdsoftOrderLineNavLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new CmdsoftOrderLineNavLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                McdsoftRefOrderlineNav = sqlReader.GetValue(1) as Guid?
            };
        }
    }
}