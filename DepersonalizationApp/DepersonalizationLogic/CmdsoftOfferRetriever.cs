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
            sb.AppendLine(" where offer.cmdsoft_offerId in (select offerIn.cmdsoft_offerId");
            sb.AppendLine("  from dbo.cmdsoft_offer as offerIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("offerIn.mcdsoft_offer2", specificationIds);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("offerIn.CreatedOn", "desc", 0, 250);
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