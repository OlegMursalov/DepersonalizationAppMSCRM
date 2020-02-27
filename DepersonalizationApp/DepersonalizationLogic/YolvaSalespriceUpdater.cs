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
    /// Обновление Прайс-листов NAV
    /// </summary>
    public class YolvaSalespriceUpdater : BaseUpdater<yolva_salesprice>
    {
        public YolvaSalespriceUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select ySlPrc.yolva_salespriceId, ySlPrc.yolva_description, ySlPrc.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from yolva_salesprice as ySlPrc");
            sb.AppendLine(" where offer.yolva_salespriceId in (select ySlPrcIn.yolva_salespriceId");
            sb.AppendLine("  from dbo.yolva_salesprice as ySlPrcIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ySlPrcIn.yolva_salespriceId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("ySlPrcIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        protected override IEnumerable<yolva_salesprice> ChangeByRules(IEnumerable<yolva_salesprice> yolvaSalesprices)
        {
            return yolvaSalesprices;
        }

        protected override yolva_salesprice ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new yolva_salesprice
            {
                Id = (Guid)sqlReader.GetValue(0),
                yolva_is_depersonalized = sqlReader.GetValue(1) as bool?
            };
        }

        protected override Entity GetEntityForUpdate(yolva_salesprice yolvaSalesprice)
        {
            var entityForUpdate = new Entity(yolvaSalesprice.LogicalName, yolvaSalesprice.Id);
            return entityForUpdate;
        }
    }
}