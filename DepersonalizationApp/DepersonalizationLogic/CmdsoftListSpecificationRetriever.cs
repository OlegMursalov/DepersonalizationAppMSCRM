using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftListSpecificationRetriever : Base<Guid>
    {
        public CmdsoftListSpecificationRetriever(SqlConnection sqlConnection, IEnumerable<Guid> specificationIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select listSp.cmdsoft_listspecificationId");
            sb.AppendLine(" from dbo.cmdsoft_listspecification as listSp");
            sb.AppendLine(" where listSp.cmdsoft_listspecificationId in (select listSpIn.cmdsoft_listspecificationId");
            sb.AppendLine("  from dbo.cmdsoft_listspecification as listSpIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("listSpIn.cmdsoft_specification", specificationIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("listSpIn.CreatedOn", "desc", 0, 250);
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