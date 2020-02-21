using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class RelatedPhoneCallDeleter : BaseDeleter<PhoneCall>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedPhoneCallDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            PhoneCall[] phoneCalls = null;
            foreach (var regObjId in _regardingObjectIds)
            {
                try
                {
                    phoneCalls = (from phoneCall in _serviceContext.PhoneCallSet
                                  where phoneCall.RegardingObjectId != null && phoneCall.RegardingObjectId.Id == regObjId
                                  select phoneCall).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedPhoneCallDeleter.Process query is failed", ex);
                }
                if (phoneCalls != null && phoneCalls.Length > 0)
                {
                    AllDelete(phoneCalls);
                }
            }
        }
    }
}