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
    /// Обновление строк Прайс-листов NAV
    /// </summary>
    public class YolvaSalespriceLineUpdater : BaseUpdater<yolva_SalesPriceLine>
    {
        public YolvaSalespriceLineUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] yolvaSalespriceIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select ySlPrcLine.yolva_salespricelineId, ySlPrcLine.yolva_amount, ySlPrcLine.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from yolva_salespriceline as ySlPrcLine");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ySlPrcLine.yolva_salespriceid", yolvaSalespriceIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override IEnumerable<yolva_SalesPriceLine> ChangeByRules(IEnumerable<yolva_SalesPriceLine> yolvaSalesPriceLines)
        {
            var randomHelper = new RandomHelper();
            foreach (var yolvaSalesPriceLine in yolvaSalesPriceLines)
            {
                yolvaSalesPriceLine.yolva_amount = randomHelper.GetDecimal(1000, 1000);
                yield return yolvaSalesPriceLine;
            }
        }

        protected override yolva_SalesPriceLine ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new yolva_SalesPriceLine
            {
                Id = (Guid)sqlReader.GetValue(0),
                yolva_amount = sqlReader.GetValue(1) as decimal?,
                yolva_is_depersonalized = sqlReader.GetValue(2) as bool?
            };
        }

        protected override Entity GetEntityForUpdate(yolva_SalesPriceLine yolvaSalesPriceLine)
        {
            var entityForUpdate = new Entity(yolvaSalesPriceLine.LogicalName, yolvaSalesPriceLine.Id);
            entityForUpdate["yolva_amount"] = yolvaSalesPriceLine.yolva_amount;
            return entityForUpdate;
        }
    }
}