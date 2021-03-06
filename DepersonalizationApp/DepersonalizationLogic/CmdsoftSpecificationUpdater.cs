﻿using CRMEntities;
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
        public CmdsoftSpecificationUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select sp.cmdsoft_specificationId, sp.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.cmdsoft_specification as sp");
            sb.AppendLine(" where sp.cmdsoft_specificationId in (select spIn.cmdsoft_specificationId");
            sb.AppendLine("  from dbo.cmdsoft_specification as spIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("spIn.cmdsoft_specificationId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("spIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }
        
        protected override cmdsoft_specification ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            var cmdsoft_specification = new cmdsoft_specification
            {
                Id = (Guid)sqlReader.GetValue(0),
                yolva_is_depersonalized = sqlReader.GetValue(1) as bool?
            };
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