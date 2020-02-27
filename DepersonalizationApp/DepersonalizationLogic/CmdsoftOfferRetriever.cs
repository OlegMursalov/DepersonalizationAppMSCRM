using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class CmdsoftOfferRetriever : Base<Guid>
    {
        public CmdsoftOfferRetriever(SqlConnection sqlConnection, IEnumerable<Guid> specificationIds) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select offer.cmdsoft_offerId");
            sb.AppendLine(" from dbo.cmdsoft_offer as offer");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("offer.mcdsoft_offer2", specificationIds);
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