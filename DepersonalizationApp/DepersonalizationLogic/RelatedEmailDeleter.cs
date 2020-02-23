using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedEmailDeleter : BaseDeleter<Email>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedEmailDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "email";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.Email", regardingObjectIds);
        }
    }
}