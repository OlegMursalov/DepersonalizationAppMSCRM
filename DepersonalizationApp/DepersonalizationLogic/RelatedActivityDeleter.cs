﻿using CRMEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepersonalizationApp.DepersonalizationLogic
{
    /// <summary>
    /// Данный класс общий
    /// </summary>
    public class RelatedActivityDeleter : BaseDeleter<ActivityPointer>
    {
        protected IEnumerable<Guid> _regardingObjectIds;

        public RelatedActivityDeleter(OrganizationServiceCtx serviceContext, IEnumerable<Guid> regardingObjectIds) : base(serviceContext)
        {
            _regardingObjectIds = regardingObjectIds;
        }

        public void Process()
        {
            foreach (var regObjId in _regardingObjectIds)
            {
                ActivityPointer[] activities = null;
                try
                {
                    activities = (from activityPointer in _serviceContext.ActivityPointerSet
                                  where activityPointer.RegardingObjectId != null && activityPointer.RegardingObjectId.Id == regObjId
                                  select activityPointer).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.Error("RelatedActivityDeleter.Process query is failed", ex);
                }
                if (activities != null && activities.Length > 0)
                {
                    AllDelete(activities);
                }
            }
        }
    }
}