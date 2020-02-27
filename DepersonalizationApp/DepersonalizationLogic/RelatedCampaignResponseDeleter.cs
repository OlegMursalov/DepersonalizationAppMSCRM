using CRMEntities;
using DepersonalizationApp.Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Удаление связанных ответов кампании
    /// </summary>
    public class RelatedCampaignResponseDeleter : BaseDeleter<CampaignResponse>
    {
        public RelatedCampaignResponseDeleter(IOrganizationService orgService, SqlConnection sqlConnection, IEnumerable<Guid> regardingObjectIds) : base(orgService, sqlConnection)
        {
            _entityLogicalName = "campaignresponse";
            _retrieveSqlQuery = SqlQueryHelper.GetQueryOfActivityGuidsByRegardingObjectIds("dbo.CampaignResponse", regardingObjectIds);
        }
    }
}