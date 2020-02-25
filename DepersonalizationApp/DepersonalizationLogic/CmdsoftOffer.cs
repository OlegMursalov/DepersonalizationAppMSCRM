using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Обновление коммерческих предложений
    /// </summary>
    public class CmdsoftOffer : BaseUpdater<cmdsoft_offer>
    {
        public CmdsoftOffer(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] accountIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select acc.AccountId, acc.Name, acc.Telephone1, acc.EMailAddress1, acc.WebSiteURL,");
            sb.AppendLine(" acc.Address1_PostalCode, acc.Description, acc.cmdsoft_inn, acc.ParentAccountId");
            sb.AppendLine(" from dbo.Account as acc");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("acc.AccountId", accountIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override void ChangeByRules(IEnumerable<cmdsoft_offer> offers)
        {
            foreach (var offer in offers)
            {
                
            }
        }

        protected override cmdsoft_offer ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new cmdsoft_offer
            {

            };
        }
    }
}