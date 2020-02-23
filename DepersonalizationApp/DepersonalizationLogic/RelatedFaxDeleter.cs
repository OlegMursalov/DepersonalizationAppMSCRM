using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedFaxDeleter : BaseDeleter<Fax>
    {
        public RelatedFaxDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "fax";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.Fax", regardingObjectIds);
        }
    }
}