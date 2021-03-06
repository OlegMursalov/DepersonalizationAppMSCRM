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
    public class CmdsoftListSpecificationUpdater : BaseUpdater<cmdsoft_listspecification>
    {
        public CmdsoftListSpecificationUpdater(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> ids) : base(orgService, sqlConnection)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"select listSp.cmdsoft_listspecificationId, listSp.{_isDepersonalizationFieldName}");
            sb.AppendLine(" from dbo.cmdsoft_listspecification as listSp");
            sb.AppendLine(" where listSp.cmdsoft_listspecificationId in (select listSpIn.cmdsoft_listspecificationId");
            sb.AppendLine("  from dbo.cmdsoft_listspecification as listSpIn");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("listSpIn.cmdsoft_listspecificationId", ids);
            sb.AppendLine(where);
            var pagination = SqlQueryHelper.GetPagination("listSpIn.CreatedOn", "desc", 0, 500);
            sb.AppendLine(pagination);
            sb.AppendLine(")");
            _retrieveSqlQuery = sb.ToString();
        }

        protected override cmdsoft_listspecification ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new cmdsoft_listspecification
            {
                Id = (Guid)sqlReader.GetValue(0),
                yolva_is_depersonalized = sqlReader.GetValue(1) as bool?
            };
        }

        protected override IEnumerable<cmdsoft_listspecification> ChangeByRules(IEnumerable<cmdsoft_listspecification> cmdsoftListSpecifications)
        {
            return cmdsoftListSpecifications;
        }

        protected override Entity GetEntityForUpdate(cmdsoft_listspecification cmdsoftListSpecification)
        {
            var entityForUpdate = new Entity(cmdsoftListSpecification.LogicalName, cmdsoftListSpecification.Id);
            return entityForUpdate;
        }
    }
}