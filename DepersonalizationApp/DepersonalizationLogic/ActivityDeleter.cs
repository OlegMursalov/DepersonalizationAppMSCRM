using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    public class ActivityDeleter : BaseDeleter<ActivityPointer>
    {
        public ActivityDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _mainQuery = from activityPointer in _serviceContext.ActivityPointerSet
                         join regObjId in regardingObjectIds on activityPointer.RegardingObjectId.Id equals regObjId
                         select activityPointer;
        }
    }
}