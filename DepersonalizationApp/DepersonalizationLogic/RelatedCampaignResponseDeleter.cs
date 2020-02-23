using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedCampaignResponseDeleter : BaseDeleter<CampaignResponse>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedCampaignResponseDeleter(IOrganizationService orgService, SqlConnection sqlConnection, Guid[] regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "campaignresponse";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.CampaignResponse", regardingObjectIds);
        }
    }
}