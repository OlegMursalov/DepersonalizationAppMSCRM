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
    /// Обновить мероприятия
    /// </summary>
    public class McdsoftSalesAppealUpdater : BaseUpdater<mcdsoft_sales_appeal>
    {
        public McdsoftSalesAppealUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select sApp.mcdsoft_sales_appealId, sApp.new_adres_text_rep, sApp.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.mcdsoft_sales_appeal as sApp");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("sApp.mcdsoft_sales_appealId", ids);
            sb.AppendLine(where);
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
                new_adres_text_rep = sqlReader.GetValue(1) as string,
                yolva_is_depersonalized = sqlReader.GetValue(2) as bool?
            };
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