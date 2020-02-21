using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedFaxDeleter : BaseDeleter<Fax>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedFaxDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            Fax[] faxes = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    faxes = (from fax in _serviceContext.FaxSet
                             where fax.RegardingObjectId != null && fax.RegardingObjectId.Id == regObjId
                             select fax).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedFaxDeleter.Process query is failed", ex);
                }
                if (faxes != null && faxes.Length > 0)
                {
                    AllDelete(faxes);
                }
            }
        }
    }
}