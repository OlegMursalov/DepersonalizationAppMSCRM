using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных задач
    /// </summary>
    public class RelatedTaskDeleter : BaseDeleter<Task>
    {
        public RelatedTaskDeleter(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "task";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.Task", regardingObjectIds);
        }
    }
}