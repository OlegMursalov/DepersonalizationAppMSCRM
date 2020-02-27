using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных email'ов
    /// </summary>
    public class RelatedEmailDeleter : BaseDeleter<Email>
    {
        public RelatedEmailDeleter(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "email";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.Email", regardingObjectIds);
        }
    }
}