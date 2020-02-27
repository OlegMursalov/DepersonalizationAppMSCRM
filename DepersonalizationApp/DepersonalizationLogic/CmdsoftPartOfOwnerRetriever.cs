using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftPartOfOwnerRetriever : Base<Guid>
    {
        public CmdsoftPartOfOwnerRetriever(SqlConnection sqlConnection, IEnumerable<Guid> opprotunityIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select partOwn.cmdsoft_part_of_ownerId");
            sb.AppendLine(" from dbo.cmdsoft_part_of_owner as partOwn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("partOwn.cmdsoft_ref_opportunity", opprotunityIds);
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