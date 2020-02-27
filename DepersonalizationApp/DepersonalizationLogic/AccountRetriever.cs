using CRMEntities;
using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class AccountLink
    {
        public Guid Id { get; set; }
        public Guid? PrimaryContactId { get; set; }
    }

    /// <summary>
    /// Извлекаем связанные с Opportunity организации
    /// </summary>
    public class AccountRetriever : Base<AccountLink>
    {
        public AccountRetriever(SqlConnection sqlConnection, IEnumerable<OpportunityLink> opportunityLinks) : base(sqlConnection)
        {
            var accountIds = new List<Guid>();

            foreach (var opportunityLink in opportunityLinks)
            {
                if (opportunityLink.CustomerId != null)
                {
                    accountIds.Add(opportunityLink.CustomerId.Value);
                }
                if (opportunityLink.CmdsoftProjectAgency != null)
                {
                    accountIds.Add(opportunityLink.CmdsoftProjectAgency.Value);
                }
                if (opportunityLink.McdsoftRefAccount != null)
                {
                    accountIds.Add(opportunityLink.McdsoftRefAccount.Value);
                }
                if (opportunityLink.CmdsoftGeneralContractor != null)
                {
                    accountIds.Add(opportunityLink.CmdsoftGeneralContractor.Value);
                }
            }

            var accountIdsDistinct = accountIds.Distinct().ToArray();

            var sb = new StringBuilder();
            sb.AppendLine("select acc.AccountId, acc.PrimaryContactId");
            sb.AppendLine(" from dbo.Account as acc");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("acc.AccountId", accountIdsDistinct);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        public List<AccountLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override AccountLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new AccountLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                PrimaryContactId = sqlReader.GetValue(1) as Guid?
            };
        }
    }
}