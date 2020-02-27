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
        public AccountRetriever(SqlConnection sqlConnection, IEnumerable<McdsoftSalesAppealLink> salesAppealLinks) : base(sqlConnection)
        {
            var accountIds = new List<Guid>();
            foreach (var salesAppealLink in salesAppealLinks)
            {
                if (salesAppealLink.McdsoftRefDealerAccount != null)
                {
                    accountIds.Add(salesAppealLink.McdsoftRefDealerAccount.Value);
                }
                if (salesAppealLink.McdsoftRefAccountClient != null)
                {
                    accountIds.Add(salesAppealLink.McdsoftRefAccountClient.Value);
                }
                if (salesAppealLink.McdsoftRefAccountAsc != null)
                {
                    accountIds.Add(salesAppealLink.McdsoftRefAccountAsc.Value);
                }
            }
            var accountIdsDistinct = accountIds.Distinct();
            _retrieveSqlQuery = SetQuery(accountIdsDistinct);
        }

        public AccountRetriever(SqlConnection sqlConnection, IEnumerable<YolvaEventsParticipantsLink> yolvaEventsParticipantsLinks) : base(sqlConnection)
        {
            var accountIds = new List<Guid>();
            foreach (var yolvaEventsParticipantsLink in yolvaEventsParticipantsLinks)
            {
                if (yolvaEventsParticipantsLink.YolvaOrganization != null)
                {
                    accountIds.Add(yolvaEventsParticipantsLink.YolvaOrganization.Value);
                }
            }
            var accountIdsDistinct = accountIds.Distinct();
            _retrieveSqlQuery = SetQuery(accountIdsDistinct);
        }

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
            var accountIdsDistinct = accountIds.Distinct();
            _retrieveSqlQuery = SetQuery(accountIdsDistinct);
        }

        private string SetQuery(IEnumerable<Guid> accountIdsDistinct)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select acc.AccountId, acc.PrimaryContactId");
            sb.AppendLine(" from dbo.Account as acc");
            sb.AppendLine(" where acc.AccountId in (select acc.AccountId");
            sb.AppendLine("  from dbo.Account as accIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("accIn.AccountId", accountIdsDistinct);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("accIn.CreatedOn", "desc", 0, 250);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            return sb.ToString();
        }

        public IEnumerable<AccountLink> Process()
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