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
    /// Обновление спецификаций
    /// </summary>
    public class CmdsoftSpecificationUpdater : BaseUpdater<cmdsoft_specification>
    {
        public CmdsoftSpecificationUpdater(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] opprotunityIds) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select sp.cmdsoft_specificationId, sp.yolva_salespricenav, sp.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.cmdsoft_specification as sp");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("sp.cmdsoft_spprojectnumber", opprotunityIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }
        
        protected override cmdsoft_specification ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var cmdsoft_specification = new cmdsoft_specification
            {
                Id = (Guid)sqlReader.GetValue(0),
                yolva_is_depersonalized = sqlReader.GetValue(2) as bool?
            };
            var yolva_salespricenavId = sqlReader.GetValue(1) as Guid?;
            if (yolva_salespricenavId != null)
            {
                cmdsoft_specification.yolva_salespricenav = new EntityReference("yolva_salesprice", yolva_salespricenavId.Value);
            }
            return cmdsoft_specification;
        }

        protected override IEnumerable<cmdsoft_specification> ChangeByRules(IEnumerable<cmdsoft_specification> cmdsoftSpecifications)
        {
            return cmdsoftSpecifications;
        }

        protected override Entity GetEntityForUpdate(cmdsoft_specification cmdsoftSpecification)
        {
            var entityForUpdate = new Entity(cmdsoftSpecification.LogicalName, cmdsoftSpecification.Id);
            return entityForUpdate;
        }
    }
}