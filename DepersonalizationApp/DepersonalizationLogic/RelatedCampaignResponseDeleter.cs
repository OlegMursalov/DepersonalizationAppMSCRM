using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedCampaignResponseDeleter : BaseDeleter<CampaignResponse>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedCampaignResponseDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            CampaignResponse[] campaignResponses = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    campaignResponses = (from campaignResponse in _serviceContext.CampaignResponseSet
                                         where campaignResponse.RegardingObjectId != null && campaignResponse.RegardingObjectId.Id == regObjId
                                         select campaignResponse).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedCampaignResponseDeleter.Process query is failed", ex);
                }
                if (campaignResponses != null && campaignResponses.Length > 0)
                {
                    AllDelete(campaignResponses);
                }
            }
        }
    }
}