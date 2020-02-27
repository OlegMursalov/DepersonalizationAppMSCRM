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
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("listSp.cmdsoft_specification", specificationIds);
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