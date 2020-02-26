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
        public YolvaSalespriceUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] specificationIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine("select ySlPrc.yolva_salespriceId, ySlPrc.yolva_description");
            sb.AppendLine(" from yolva_salesprice as ySlPrc");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("ySlPrc.yolva_salespriceId", specificationIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }

        protected override void ChangeByRules(IEnumerable<yolva_salesprice> yolvaSalesprices)
        {
        }

        protected override yolva_salesprice ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new yolva_salesprice
            {
                Id = (Guid)sqlReader.GetValue(0)
            };
        }

        protected override Entity GetEntityForUpdate(yolva_salesprice yolvaSalesprice)
        {
            var entityForUpdate = new Entity(yolvaSalesprice.LogicalName, yolvaSalesprice.Id);
            entityForUpdate[_commonDepersonalizationNameField] = true;
            return entityForUpdate;
        }
    }
}