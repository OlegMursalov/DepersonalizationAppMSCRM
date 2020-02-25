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
            sb.AppendLine("select sp.cmdsoft_specificationId");
            sb.AppendLine(" from dbo.cmdsoft_specification as sp");
            var where = SqlQueryHelper.GetPartOfQueryWhereIn("sp.cmdsoft_spprojectnumber", opprotunityIds);
            sb.AppendLine(where);
            _retrieveSqlQuery = sb.ToString();
        }
        
        protected override cmdsoft_specification ConvertSqlDataReaderItem(SqlDataReader sqlReader)
        {
            return new cmdsoft_specification
            {
                Id = (Guid)sqlReader.GetValue(0)
            };
        }

        protected override void ChangeByRules(IEnumerable<cmdsoft_specification> records)
        {
            
        }
    }
}