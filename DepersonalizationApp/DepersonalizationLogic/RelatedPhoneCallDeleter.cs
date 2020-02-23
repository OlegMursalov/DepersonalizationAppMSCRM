using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedPhoneCallDeleter : BaseDeleter<PhoneCall>
    {
        public RelatedPhoneCallDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "phonecall";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.PhoneCall", regardingObjectIds);
        }
    }
}