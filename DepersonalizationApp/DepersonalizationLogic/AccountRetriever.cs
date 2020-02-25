using CRMEntities;
using DepersonalizationApp.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class AccountSimple
    {
        public Guid AccountId { get; set; }
        public Guid? PrimaryContactId { get; set; }
    }

    /// <summary>
    /// Извлекаем связанные с Opportunity организации
    /// </summary>
    public class AccountRetriever : Base<AccountSimple>
    {
        public AccountRetriever(SqlConnection sqlConnection, IEnumerable<Opportunity> opportunities) : base(sqlConnection)
        {
            var accountIds = new List<Guid>();

            foreach (var opportunity in opportunities)
            {
                if (opportunity.CustomerId != null)
                {
                    accountIds.Add(opportunity.CustomerId.Id);
                }
                if (opportunity.cmdsoft_project_agency != null)
                {
                    accountIds.Add(opportunity.cmdsoft_project_agency.Id);
                }
                if (opportunity.mcdsoft_ref_account != null)
                {
                    accountIds.Add(opportunity.mcdsoft_ref_account.Id);
                }
                if (opportunity.cmdsoft_GeneralContractor != null)
                {
                    accountIds.Add(opportunity.cmdsoft_GeneralContractor.Id);
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

        public IEnumerable<AccountSimple> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override AccountSimple ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new AccountSimple
            {
                AccountId = (Guid)sqlReader.GetValue(0),
                PrimaryContactId = sqlReader.GetValue(1) as Guid?
            };
        }
    }
}