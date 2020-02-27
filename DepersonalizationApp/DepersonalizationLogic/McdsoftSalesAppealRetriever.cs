using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class McdsoftSalesAppealLink
    {
        public Guid Id { get; set; }

        // Контакт (contact)
        public Guid? McdsoftRefContact { get; set; }
        public Guid? McdsoftRefContactAsc { get; set; }

        // Организация (account)
        public Guid? McdsoftRefDealerAccount { get; set; }
        public Guid? McdsoftRefAccountClient { get; set; }
        public Guid? McdsoftRefAccountAsc { get; set; }

        // Проект (opportunity)
        public Guid? McdsoftRefOpportunity { get; set; }

        // Состав продаж (mcdsoft_ref_orderlinenav)
        public Guid? McdsoftRefOrderlinenav { get; set; }

        // Состав продаж (cmdsoft_ref_orderlinenav)
        public Guid? CmdsoftRefOrderlinenav { get; set; }
    }

    public class McdsoftSalesAppealRetriever : Base<McdsoftSalesAppealLink>
    {
        public McdsoftSalesAppealRetriever(SqlConnection sqlConnection) : base(sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select sApp.mcdsoft_sales_appealId, sApp.mcdsoft_ref_contact, sApp.mcdsoft_ref_dealer_account,");
            sb.AppendLine(" sApp.mcdsoft_ref_account_client, sApp.mcdsoft_ref_account_asc, sApp.cmdsoft_ref_orderlinenav,");
            sb.AppendLine(" sApp.mcdsoft_ref_opportunity, sApp.mcdsoft_ref_orderlinenav, sApp.mcdsoft_ref_contact_asc");
            sb.AppendLine(" from dbo.mcdsoft_sales_appeal as sApp");
            sb.AppendLine("  where sApp.mcdsoft_sales_appealId in (select sAppIn.mcdsoft_sales_appealId");
            sb.AppendLine("  from dbo.mcdsoft_sales_appeal as sAppIn");
            var pagination = SqlQueryHelper.GetPagination("sAppIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        public IEnumerable<McdsoftSalesAppealLink> Process()
        {
            return FastRetrieveAllItems();
        }

        protected override McdsoftSalesAppealLink ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new McdsoftSalesAppealLink
            {
                Id = (Guid)sqlReader.GetValue(0),
                McdsoftRefContact = sqlReader.GetValue(1) as Guid?,
                McdsoftRefDealerAccount = sqlReader.GetValue(2) as Guid?,
                McdsoftRefAccountClient = sqlReader.GetValue(3) as Guid?,
                McdsoftRefAccountAsc = sqlReader.GetValue(4) as Guid?,
                CmdsoftRefOrderlinenav = sqlReader.GetValue(5) as Guid?,
                McdsoftRefOpportunity = sqlReader.GetValue(6) as Guid?,
                McdsoftRefOrderlinenav = sqlReader.GetValue(7) as Guid?,
                McdsoftRefContactAsc = sqlReader.GetValue(8) as Guid?
            };
        }
    }
}