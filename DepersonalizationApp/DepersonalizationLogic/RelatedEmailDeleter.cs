using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedEmailDeleter : BaseDeleter<Email>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedEmailDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            Email[] emails = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    emails = (from email in _serviceContext.EmailSet
                              where email.RegardingObjectId != null && email.RegardingObjectId.Id == regObjId
                              select email).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedEmailDeleter.Process query is failed", ex);
                }
                if (emails != null && emails.Length > 0)
                {
                    AllDelete(emails);
                }
            }
        }
    }
}