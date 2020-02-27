using CRMEntities;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using UpdaterApp.DepersonalizationLogic;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Обновить мероприятия
    /// </summary>
    public class McdsoftSalesAppealUpdater : BaseUpdater<mcdsoft_sales_appeal>
    {
        public McdsoftSalesAppealUpdater(IOrganizationService orgService, SqlConnection sqlConnection) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select sApp.mcdsoft_sales_appealId, sApp.mcdsoft_ref_contact, sApp.mcdsoft_ref_dealer_account,");
            sb.AppendLine(" sApp.mcdsoft_ref_account_client, sApp.new_adres_text_rep, sApp.mcdsoft_ref_account_asc,");
            sb.AppendLine($" sApp.mcdsoft_ref_opportunity, sApp.mcdsoft_ref_orderlinenav, sApp.mcdsoft_ref_contact_asc, sApp.mcdsoft_ref_contact_asc, sApp.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.mcdsoft_sales_appeal as sApp");
            sb.AppendLine("  where sApp.mcdsoft_sales_appealId in (select sAppIn.mcdsoft_sales_appealId");
            sb.AppendLine("  from dbo.mcdsoft_sales_appeal as sAppIn");
            sb.AppendLine("  order by sAppIn.CreatedOn desc");
            sb.AppendLine("  offset 0 rows");
            sb.AppendLine("  fetch next 500 rows only)");
             _retrieveSqlQuery = sb.ToString();
        }

        protected override IEnumerable<mcdsoft_sales_appeal> ChangeByRules(IEnumerable<mcdsoft_sales_appeal> salesAppeals)
        {
            foreach (var salesAppeal in salesAppeals)
            {
                salesAppeal.new_adres_text_rep = null;
                yield return salesAppeal;
            }
        }

        protected override mcdsoft_sales_appeal ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var mcdsoft_sales_appeal = new mcdsoft_sales_appeal
            {
                Id = (Guid)sqlReader.GetValue(0),
                new_adres_text_rep = sqlReader.GetValue(4) as string,
                yolva_is_depersonalized = sqlReader.GetValue(10) as bool?
            };
            var mcdsoft_ref_contactId = sqlReader.GetValue(1) as Guid?;
            if (mcdsoft_ref_contactId != null)
            {
                mcdsoft_sales_appeal.mcdsoft_ref_contact = new EntityReference("contact", mcdsoft_ref_contactId.Value);
            }
            var mcdsoft_ref_dealer_accountId = sqlReader.GetValue(2) as Guid?;
            if (mcdsoft_ref_dealer_accountId != null)
            {
                mcdsoft_sales_appeal.mcdsoft_ref_dealer_account = new EntityReference("account", mcdsoft_ref_dealer_accountId.Value);
            }
            var mcdsoft_ref_account_clientId = sqlReader.GetValue(3) as Guid?;
            if (mcdsoft_ref_account_clientId != null)
            {
                mcdsoft_sales_appeal.mcdsoft_ref_account_client = new EntityReference("account", mcdsoft_ref_account_clientId.Value);
            }
            var mcdsoft_ref_account_ascId = sqlReader.GetValue(5) as Guid?;
            if (mcdsoft_ref_account_ascId != null)
            {
                mcdsoft_sales_appeal.mcdsoft_ref_account_asc = new EntityReference("account", mcdsoft_ref_account_ascId.Value);
            }
            var mcdsoft_ref_opportunityId = sqlReader.GetValue(6) as Guid?;
            if (mcdsoft_ref_opportunityId != null)
            {
                mcdsoft_sales_appeal.mcdsoft_ref_opportunity = new EntityReference("opportunity", mcdsoft_ref_opportunityId.Value);
            }
            var mcdsoft_ref_orderlinenavId = sqlReader.GetValue(7) as Guid?;
            if (mcdsoft_ref_orderlinenavId != null)
            {
                mcdsoft_sales_appeal.mcdsoft_ref_orderlinenav = new EntityReference("mcdsoft_orderlinenav", mcdsoft_ref_orderlinenavId.Value);
            }
            var cmdsoft_ref_orderlinenavId = sqlReader.GetValue(8) as Guid?;
            if (cmdsoft_ref_orderlinenavId != null)
            {
                mcdsoft_sales_appeal.cmdsoft_ref_orderlinenav = new EntityReference("cmdsoft_orderlinenav", cmdsoft_ref_orderlinenavId.Value);
            }
            var mcdsoft_ref_contact_ascId = sqlReader.GetValue(9) as Guid?;
            if (mcdsoft_ref_contact_ascId != null)
            {
                mcdsoft_sales_appeal.mcdsoft_ref_contact_asc = new EntityReference("contact", mcdsoft_ref_contact_ascId.Value);
            }
            return mcdsoft_sales_appeal;
        }

        protected override Entity GetEntityForUpdate(mcdsoft_sales_appeal salesAppeal)
        {
            var entityForUpdate = new Entity(salesAppeal.LogicalName, salesAppeal.Id);
            entityForUpdate["new_adres_text_rep"] = salesAppeal.new_adres_text_rep;
            return entityForUpdate;
        }
    }
}